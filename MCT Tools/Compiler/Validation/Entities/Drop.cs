using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Entities
{
    /// <summary>
    /// An element of the npc.drop.choose array.
    /// </summary>
    public class Choice(ModCompiler mc) : ValidatorObject(mc)
    {
#pragma warning disable 1591
        public string item  ;
        public float  weight;
        public int[]  stack ;
#pragma warning restore 1591

        internal Drop d;

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "item", ref item), errors);
            AddIfNotNull(SetJsonValue(json, "weight", ref weight, 1f), errors);

            AddIfNotNull(d.ValidateStack(Building, json, ref stack), errors);
            //AddIfNotNull(SetJsonValue(json, "stack", ref stack, 1), errors);

            return errors;
        }
    }

    /// <summary>
    /// An NPC drop.
    /// </summary>
    public class Drop(ModCompiler mc) : ValidatorObject(mc)
    {
        readonly static int[] TwoOnes = { 1, 1 };

#pragma warning disable 1591
        public string item = String.Empty;
        public int[] stack;
        public float chance = 0f;

        public bool usesChoose = false;
        public Choice[] choose;
#pragma warning restore 1591

        internal IEnumerable<CompilerError> ValidateStack(ModData Building, JsonFile json, ref int[] stack)
        {
            List<CompilerError> errors = new List<CompilerError>();

            if (!json.Json.Has("stack"))
            {
                stack = new[] { 1 };

                return errors;
            }

            JsonData jStack = json.Json["stack"];

            if (jStack.IsArray)
            {
                AddIfNotNull(SetJsonValue(json, "stack", ref stack, true, TwoOnes), errors);

                if (stack.Length != 2)
                    errors.Add(new CompilerError(Building)
                    {
                        Cause = new IndexOutOfRangeException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "Invalid length " + stack.Length + " for field 'stack'."
                    });
                if (stack[0] > stack[1])
                    errors.Add(new CompilerError(Building)
                    {
                        Cause = new ArgumentOutOfRangeException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "The minimum value of the stack cannot be greater than the maximum value."
                    });
                if (stack[0] < 0 || stack[1] < 0)
                    errors.Add(new CompilerError(Building)
                    {
                        Cause = new ArgumentOutOfRangeException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "The stack values must be greater than or equal to 0."
                    });
            }
            else
            {
                int st = 1;
                AddIfNotNull(SetJsonValue(json, "stack", ref st, 1), errors);
                stack = new[] { st };

                if (st <= 0)
                    errors.Add(new CompilerError(Building)
                    {
                        Cause = new ArgumentOutOfRangeException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "'stack' is equal to or below 0. Please remove the Drop object from the array, or change the stack."
                    });
            }


            return errors;
        }

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            if (json.Json.Has("choose"))
            {
                JsonData jChoose = json.Json["choose"];

                usesChoose = true;

                if (!jChoose.IsArray)
                    errors.Add(new CompilerError(Building)
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "'choose' has to be an array of objects."
                    });

                choose = new Choice[jChoose.Count];
                for (int i = 0; i < jChoose.Count; i++)
                {
                    if (!jChoose[i].IsObject)
                        errors.Add(new CompilerError(Building)
                        {
                            Cause = new ArrayTypeMismatchException(),
                            FilePath = json.Path,
                            IsWarning = false,
                            Message = "'choose' has to be an array of objects."
                        });

                    Choice c = new Choice(Compiler);
                    c.d = this;
                    AddIfNotNull(c.CreateAndValidate(new JsonFile(json.Path, jChoose[i])), errors);
                    c.d = null;
                    choose[i] = c;
                }
            }
            else
            {
                ValidateStack(Building, json, ref stack);

                #region stack
                /*if (json.Json.Has("stack"))
                {
                    JsonData jStack = json.Json["stack"];

                    if (jStack.IsArray)
                    {
                        AddIfNotNull(SetJsonValue(json, "stack", ref stack, true, TwoOnes), errors);

                        if (stack.Length != 2)
                            errors.Add(new CompilerError(Building)
                            {
                                Cause = new IndexOutOfRangeException(),
                                FilePath = json.Path,
                                IsWarning = false,
                                Message = "Invalid length " + stack.Length + " for field 'stack'."
                            });
                        if (stack[0] > stack[1])
                            errors.Add(new CompilerError(Building)
                            {
                                Cause = new ArgumentOutOfRangeException(),
                                FilePath = json.Path,
                                IsWarning = false,
                                Message = "The minimum value of the stack cannot be greater than the maximum value."
                            });
                        if (stack[0] < 0 || stack[1] < 0)
                            errors.Add(new CompilerError(Building)
                            {
                                Cause = new ArgumentOutOfRangeException(),
                                FilePath = json.Path,
                                IsWarning = false,
                                Message = "The stack values must be greater than or equal to 0."
                            });
                    }
                    else
                    {
                        int st = 1;
                        AddIfNotNull(SetJsonValue(json, "stack", ref st, 1), errors);
                        stack = new[] { st };

                        if (st <= 0)
                            errors.Add(new CompilerError(Building)
                            {
                                Cause = new ArgumentOutOfRangeException(),
                                FilePath = json.Path,
                                IsWarning = false,
                                Message = "'stack' is equal to or below 0. Please remove the Drop object from the array, or change the stack."
                            });
                    }
                }*/
                #endregion

                AddIfNotNull(SetJsonValue(json, "item", ref item), errors);
            }

            AddIfNotNull(SetJsonValue(json, "chance", ref chance, 1f), errors);
            if (chance > 1f)
                errors.Add(new CompilerError(Building)
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = true,
                    Message = "'chance' is above 1. Set it to 1."
                });
            if (chance <= 0f)
                errors.Add(new CompilerError(Building)
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = true,
                    Message = "'chance' is equal to or below 0. Please remove the Drop object from the array, or change it."
                });

            return errors;
        }
    }
}
