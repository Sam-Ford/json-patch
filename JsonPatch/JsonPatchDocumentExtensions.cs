using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace JsonPatch
{
    public static class JsonPatchDocumentExtensions
    {
        /// <summary>
        /// "Diff" method to populate a <c>JsonPatchDocument</c> with operations which if applied to <c> original </c> would generate <c> modified </c>
        /// <param name="patch">The patch to populate with operations</param>
        /// <param name="original">The original object</param>
        /// <param name="modified">The modified object</param>
        ///</summary>
        public static void Populate(this JsonPatchDocument patch, object original, object modified) {
            var originalObj = JObject.FromObject(original);
            var modifiedObj = JObject.FromObject(modified);
            HandleObject(originalObj, modifiedObj, patch, "/");
            
            if (!patch.Operations.Any()) {
                return;
            }

            foreach (var operation in patch.Operations) {
                operation.path = operation.path.ToLower();
            }
        }

        private static void HandleObject(JObject original, JObject modified, JsonPatchDocument patch, string path)
        {
            var originalPropertyNames = original.Properties().Select(x => x.Name).ToArray();
            var modifiedPropertyNames = modified.Properties().Select(x => x.Name).ToArray();

            foreach (var name in originalPropertyNames.Except(modifiedPropertyNames))
            {
                patch.Remove($"{path}{name}");
            }

            foreach (var name in modifiedPropertyNames.Except(originalPropertyNames))
            {
                var property = modified.Property(name);
                patch.Add($"{path}{name}", property.Value);
            }

            foreach (var name in originalPropertyNames.Intersect(modifiedPropertyNames)) {
                var originalProperty = original.Property(name);
                var modifiedProperty = modified.Property(name);

                HandleReplace(originalProperty.Value, modifiedProperty.Value, patch, $"{path}{name}/");
            }
        }

        private static void HandleReplace(JToken original, JToken modified, JsonPatchDocument patch, string path) {
            if (original.Type != modified.Type)
            {
                patch.Replace(path.TrimEnd('/'), modified);
            }
            else 
            {
                if (Equal(original, modified)) {
                    return;
                }

                switch (original.Type) {
                    case JTokenType.Object:
                        HandleObject(original as JObject, modified as JObject, patch, path);
                        break;
                    case JTokenType.Array:
                        HandleArray(original as JArray, modified as JArray, patch, path);
                        break;
                    default:
                        patch.Replace(path.TrimEnd('/'), modified);
                        break;

                }
            }
        }

        private static bool Equal(JToken original, JToken modified) {
            return string.Equals(original.ToString(Formatting.None), modified.ToString(Formatting.None));
        }

        private static void HandleArray(JArray original, JArray modified, JsonPatchDocument patch, string path) {
            var originalChildren = original.Children().ToList();
            var modifiedChildren = modified.Children().ToList();    
            
            if (original.Count >= modified.Count) {
                for (int i = 0; i < originalChildren.Count(); i++) {
                    var originalChild = originalChildren[i];
                    if (i < modifiedChildren.Count())
                    {
                        HandleReplace(originalChild, modifiedChildren[i], patch, $"{path}{i}/");
                    }
                    else {
                        patch.Remove($"{path}{i}");
                    } 
                }
            } else {
                for (int i = 0; i < modifiedChildren.Count(); i++) {
                    var modifiedChild = modifiedChildren[i];
                    if (i < originalChildren.Count())
                    {
                        HandleReplace(originalChildren[i], modifiedChild, patch, $"{path}{i}/");
                    }
                    else {
                        patch.Add($"{path}-", modifiedChild);
                    }    
                }
            }
        }
    }
}
