﻿using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace PoroCYon.MCT.Tools.Compiler
{
    /// <summary>
    /// A JsonData object paired with its file path.
    /// </summary>
    public struct JsonFile
    {
        /// <summary>
        /// Gets the path to the JSON file
        /// </summary>
        public string Path;
        /// <summary>
        /// Gets the JSON object.
        /// </summary>
        public JsonData Json;

        /// <summary>
        /// Gets whether the JsonFile is an empty instance or not.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return Path == null || Json == null;
            }
        }

        /// <summary>
        /// Creates a new instance of the JsonFile class
        /// </summary>
        /// <param name="path">The path to the JSON file.</param>
        /// <param name="json">The JSON object.</param>
        public JsonFile(string path, JsonData json)
        {
            Path = path;
            Json = json;
        }

        /// <summary>
        /// Casts a JsonFile to a JsonData objects.
        /// </summary>
        /// <param name="file">The JsonFile to cast.</param>
        public static implicit operator JsonData(JsonFile file)
        {
            return file.Json;
        }
    }
}
