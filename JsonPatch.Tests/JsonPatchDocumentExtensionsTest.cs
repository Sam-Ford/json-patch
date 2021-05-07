using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using JsonPatch;

namespace JsonPatch.Tests
{
    public class FlatObject {
        public string StringValue { get; set; }

        public int IntegerValue { get; set; }
    }

    public class SimpleArrayObject {
        public List<string> StringArrayValue { get; set; }
    }

    public class NestedObject {
        public List<FlatObject> ObjectArrayValue { get; set; }
    }

    public class JsonPatchDocumentExtensionsTest
    {
        
        [Fact]
        public void FlatObject_Replace()
        {
            var random = new Random();
            var original = new FlatObject { StringValue = Guid.NewGuid().ToString(), IntegerValue = random.Next() };
            var modified = new FlatObject { StringValue = original.StringValue, IntegerValue = original.IntegerValue };
            var expected = new JsonPatchDocument();
            expected.Replace($"/{nameof(FlatObject.StringValue)}", Guid.NewGuid().ToString());
            expected.Replace($"/{nameof(FlatObject.IntegerValue)}", random.Next());
            expected.ApplyTo(modified);   

            var actual = new JsonPatchDocument();
            actual.Populate(original, modified);
            AssertPatchesMatch(expected, actual);
        }

        [Fact]
        public void SimpleArrayObject_AddObject()
        {
            var random = new Random();
            var original = new SimpleArrayObject { 
                 StringArrayValue = new List<string> {
                     "a",
                     "b",
                     "c"
                 }
            };
            var modified = new SimpleArrayObject { 
                 StringArrayValue = new List<string> {
                     "a",
                     "b",
                     "c"
                 }
            };
            var expected = new JsonPatchDocument();
            expected.Add($"/{nameof(SimpleArrayObject.StringArrayValue)}/-", Guid.NewGuid().ToString());
            expected.ApplyTo(modified);   

            var actual = new JsonPatchDocument();
            actual.Populate(original, modified);
            AssertPatchesMatch(expected, actual);
        }

        [Fact]
        public void SimpleArrayObject_RemoveObject()
        {
            var random = new Random();
            var original = new SimpleArrayObject { 
                 StringArrayValue = new List<string> {
                     "a",
                     "b",
                     "c"
                 }
            };
            var modified = new SimpleArrayObject { 
                 StringArrayValue = new List<string> {
                     "a",
                     "b",
                     "c"
                 }
            };
            var expected = new JsonPatchDocument();
            expected.Remove($"/{nameof(SimpleArrayObject.StringArrayValue)}/2");
            expected.ApplyTo(modified);   

            var actual = new JsonPatchDocument();
            actual.Populate(original, modified);
            AssertPatchesMatch(expected, actual);
        }

        [Fact]
        public void SimpleArrayObject_ReplaceObject()
        {
            var random = new Random();
            var original = new SimpleArrayObject { 
                 StringArrayValue = new List<string> {
                     "a",
                     "b",
                     "c"
                 }
            };
            var modified = new SimpleArrayObject { 
                 StringArrayValue = new List<string> {
                     "a",
                     "b",
                     "c"
                 }
            };
            var expected = new JsonPatchDocument();
            expected.Replace($"/{nameof(SimpleArrayObject.StringArrayValue)}/0", "test");
            Assert.Equal(modified.StringArrayValue.Count(), 3);
            expected.ApplyTo(modified);   

            var actual = new JsonPatchDocument();
            actual.Populate(original, modified);
            AssertPatchesMatch(expected, actual);
        }

        
        [Fact]
        public void NestedObject_AddSubObject()
        {
            var random = new Random();
            var original = new NestedObject { 
                 ObjectArrayValue = new List<FlatObject> {
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     },
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     },
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     }
                 }
            };
            var modified = new NestedObject { 
                 ObjectArrayValue = new List<FlatObject> {
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     },
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     },
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     }
                 }
            };
            var expected = new JsonPatchDocument();
            expected.Add($"/{nameof(NestedObject.ObjectArrayValue)}/-", new FlatObject { StringValue = "d", IntegerValue = 4 });
            expected.ApplyTo(modified);   

            var actual = new JsonPatchDocument();
            actual.Populate(original, modified);
            AssertPatchesMatch(expected, actual);
        }

        [Fact]
        public void NestedObject_RemoveSubObject()
        {
            var random = new Random();
            var original = new NestedObject { 
                 ObjectArrayValue = new List<FlatObject> {
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     },
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     },
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     }
                 }
            };
            var modified = new NestedObject { 
                 ObjectArrayValue = new List<FlatObject> {
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     },
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     },
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     }
                 }
            };
            var expected = new JsonPatchDocument();
            expected.Remove($"/{nameof(NestedObject.ObjectArrayValue)}/1");
            expected.ApplyTo(modified);   

            var actual = new JsonPatchDocument();
            actual.Populate(original, modified);
            AssertPatchesMatch(expected, actual);
        }

                [Fact]
        public void NestedObject_ReplaceSubObject()
        {
            var random = new Random();
            var original = new NestedObject { 
                 ObjectArrayValue = new List<FlatObject> {
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     },
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     },
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     }
                 }
            };
            var modified = new NestedObject { 
                 ObjectArrayValue = new List<FlatObject> {
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     },
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     },
                     new FlatObject {
                         StringValue = "a",
                         IntegerValue = 1
                     }
                 }
            };
            var expected = new JsonPatchDocument();
            expected.Replace($"/{nameof(NestedObject.ObjectArrayValue)}/1/StringValue", "d");
            expected.Replace($"/{nameof(NestedObject.ObjectArrayValue)}/1/IntegerValue", 4);
            expected.ApplyTo(modified);   

            var actual = new JsonPatchDocument();
            actual.Populate(original, modified);
            AssertPatchesMatch(expected, actual);
        }

        private void AssertPatchesMatch(JsonPatchDocument expected, JsonPatchDocument actual) {
            var expectedOperations = expected.Operations;
            var actualOperations = actual.Operations;
            Assert.Equal(expectedOperations.Count, actualOperations.Count);
            foreach (var operation in expectedOperations) {
                actualOperations.Any((o) => OperationsMatch(operation, o));
            }
        }

        private bool OperationsMatch(Operation expected, Operation actual) {
            if (expected.from != actual.from) {
                return false;
            }

            if (expected.op != actual.op) {
                return false;
            }

            if (expected.OperationType != actual.OperationType) {
                return false;
            }

            if (expected.path != actual.path) {
                return false;
            }

            if (expected.value != actual.value) {
                return false;
            }

            return true;
        }
    }
}
