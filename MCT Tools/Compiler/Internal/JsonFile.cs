using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace PoroCYon.MCT.Tools.Internal
{
    class JsonFile
    {
        internal string path;
        internal JsonData json;

        internal JsonFile(string path, JsonData json)
        {
            this.path = path;
            this.json = json;
        }

        public static implicit operator JsonData(JsonFile file)
        {
            return file.json;
        }
        public static implicit operator string  (JsonFile file)
        {
            return file.path;
        }
    }
}
