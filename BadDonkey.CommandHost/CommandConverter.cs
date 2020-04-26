using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BadDonkey.CommandHost
{
    public class CommandConverter : JsonConverter
    {
        private readonly Assembly _commandAssembly;

        public CommandConverter(Assembly commandAssembly)
        {
            _commandAssembly = commandAssembly;
        }
        
        public override bool CanConvert(Type objectType)
        {
            return typeof(Command).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            var commandType = _commandAssembly.GetTypes().Single(s => s.Name.StartsWith($"{jo["kind"].Value<string>()}Command"));

            var obj = Activator.CreateInstance(commandType);

            serializer.Populate(jo.CreateReader(), obj);

            return obj;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}