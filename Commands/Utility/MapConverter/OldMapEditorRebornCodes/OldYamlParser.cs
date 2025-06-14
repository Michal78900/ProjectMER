using Serialization;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;
using YamlDotNet.Serialization.NodeDeserializers;
using YamlDotNet.Serialization.ObjectGraphVisitors;
using YamlDotNet.Serialization.TypeInspectors;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes
{
    public static class OldYamlParser
    {
        public static ISerializer Serializer { get; set; } = new SerializerBuilder()
            .WithTypeConverter(new VectorsConverter())
            .WithTypeConverter(new ColorConverter())
            //.WithTypeConverter(new AttachmentIdentifiersConverter())
            .WithEventEmitter(eventEmitter => new TypeAssigningEventEmitter(eventEmitter))
            .WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
            .WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor))
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .DisableAliases()
            .Build();
        public static IDeserializer Deserializer { get; set; } = new DeserializerBuilder()
            .WithTypeConverter(new VectorsConverter())
            .WithTypeConverter(new ColorConverter())
            //.WithTypeConverter(new AttachmentIdentifiersConverter())
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), deserializer => deserializer.InsteadOf<ObjectNodeDeserializer>())
            .IgnoreFields()
            .IgnoreUnmatchedProperties()
            .Build();
    }
    public class VectorsConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4);
        public object ReadYaml(IParser parser, Type type)
        {
            if (!parser.TryConsume<MappingStart>(out _))
                throw new InvalidDataException($"Cannot deserialize object of type {type.FullName}.");
            List<float> coordinates = new List<float>();
            int i = 0;
            while (!parser.TryConsume<MappingEnd>(out _))
            {
                if (i++ % 2 == 0)
                {
                    parser.MoveNext();
                    continue;
                }
                if (!parser.TryConsume(out Scalar scalar) || !float.TryParse(scalar.Value, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out float coordinate))
                {
                    throw new InvalidDataException($"Invalid float value.");
                }
                coordinates.Add(coordinate);
            }
            if (type == typeof(Vector2))
            {
                return new Vector2(coordinates[0], coordinates[1]);
            }
            else if (type == typeof(Vector3))
            {
                return new Vector3(coordinates[0], coordinates[1], coordinates[2]);
            }
            else if (type == typeof(Vector4))
            {
                return new Vector4(coordinates[0], coordinates[1], coordinates[2], coordinates[3]);
            }

            throw new InvalidDataException("Unsupported vector type.");
        }
        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            Dictionary<string, float> coordinates = new Dictionary<string, float>();
            if (value is Vector2 vector2)
            {
                coordinates["x"] = vector2.x;
                coordinates["y"] = vector2.y;
            }
            else if (value is Vector3 vector3)
            {
                coordinates["x"] = vector3.x;
                coordinates["y"] = vector3.y;
                coordinates["z"] = vector3.z;
            }
            else if (value is Vector4 vector4)
            {
                coordinates["x"] = vector4.x;
                coordinates["y"] = vector4.y;
                coordinates["z"] = vector4.z;
                coordinates["w"] = vector4.w;
            }
            emitter.Emit(new MappingStart());
            foreach (KeyValuePair<string, float> coordinate in coordinates)
            {
                emitter.Emit(new Scalar(coordinate.Key));
                emitter.Emit(new Scalar(coordinate.Value.ToString(CultureInfo.GetCultureInfo("en-US"))));
            }
            emitter.Emit(new MappingEnd());
        }
    }
    public class ColorConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => type == typeof(Color);
        public object ReadYaml(IParser parser, Type type)
        {
            if (!parser.TryConsume<MappingStart>(out _))
                throw new InvalidDataException($"Cannot deserialize object of type {type.FullName}");
            List<float> coordinates = new List<float>();
            int i = 0;
            while (!parser.TryConsume<MappingEnd>(out _))
            {
                if (i++ % 2 == 0)
                {
                    parser.MoveNext();
                    continue;
                }
                if (!parser.TryConsume(out Scalar scalar) || !float.TryParse(scalar.Value, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out float coordinate))
                {
                    throw new InvalidDataException("Invalid float value.");
                }
                coordinates.Add(coordinate);
            }
            return Activator.CreateInstance(type, coordinates.ToArray());
        }
        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            Dictionary<string, float> coordinates = new Dictionary<string, float>();
            if (value is Color color)
            {
                coordinates["r"] = color.r;
                coordinates["g"] = color.g;
                coordinates["b"] = color.b;
                coordinates["a"] = color.a;
            }
            emitter.Emit(new MappingStart());
            foreach (KeyValuePair<string, float> coordinate in coordinates)
            {
                emitter.Emit(new Scalar(coordinate.Key));
                emitter.Emit(new Scalar(coordinate.Value.ToString(CultureInfo.GetCultureInfo("en-US"))));
            }
            emitter.Emit(new MappingEnd());
        }
    }
    public class CommentsObjectGraphVisitor : ChainedObjectGraphVisitor
    {
        public CommentsObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
            : base(nextVisitor)
        {
        }
        public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
        {
            if (value is CommentsObjectDescriptor commentsDescriptor && commentsDescriptor.Comment is not null)
            {
                foreach (string subComment in commentsDescriptor.Comment.Split('\n'))
                {
                    context.Emit(new Comment(subComment, false));
                }
            }
            return base.EnterMapping(key, value, context);
        }
    }
    public class CommentGatheringTypeInspector : TypeInspectorSkeleton
    {
        private readonly ITypeInspector innerTypeDescriptor;
        public CommentGatheringTypeInspector(ITypeInspector innerTypeDescriptor)
        {
            this.innerTypeDescriptor = innerTypeDescriptor ?? throw new ArgumentNullException("innerTypeDescriptor");
        }
        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
        {
            return innerTypeDescriptor
                .GetProperties(type, container)
                .Select(descriptor => new CommentsPropertyDescriptor(descriptor));
        }
    }
    public class TypeAssigningEventEmitter : ChainedEventEmitter
    {
        private readonly char[] multiline = new char[] { '\r', '\n', '\x85', '\x2028', '\x2029' };
        public TypeAssigningEventEmitter(IEventEmitter nextEmitter)
            : base(nextEmitter)
        {
        }
        public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            if (eventInfo.Source.StaticType != typeof(object) && Type.GetTypeCode(eventInfo.Source.StaticType) == TypeCode.String && !UnderscoredNamingConvention.Instance.Properties.Contains(eventInfo.Source.Value))
            {
                if (eventInfo.Source.Value == null || eventInfo.Source.Value.ToString().IndexOfAny(multiline) is -1)
                    eventInfo.Style = ScalarStyle.SingleQuoted;
                else
                    eventInfo.Style = ScalarStyle.Literal;
            }
            base.Emit(eventInfo, emitter);
        }
    }
    public class UnderscoredNamingConvention : INamingConvention
    {
        public static UnderscoredNamingConvention Instance { get; } = new();
        public List<object> Properties { get; } = new();
        public string Apply(string value)
        {
            string newValue = ToSnakeCase(value);
            Properties.Add(newValue);
            return newValue;
        }
        public string ToSnakeCase(string str, bool shouldReplaceSpecialChars = true)
        {
            string snakeCaseString = string.Concat(str.Select((ch, i) => (i > 0) && char.IsUpper(ch) ? "_" + ch.ToString() : ch.ToString())).ToLower();
            return shouldReplaceSpecialChars ? Regex.Replace(snakeCaseString, @"[^0-9a-zA-Z_]+", string.Empty) : snakeCaseString;
        }
    }
    public class ValidatingNodeDeserializer : INodeDeserializer
    {
        private readonly INodeDeserializer nodeDeserializer;
        public ValidatingNodeDeserializer(INodeDeserializer nodeDeserializer)
        {
            this.nodeDeserializer = nodeDeserializer;
        }
        public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            try
            {
                if (nodeDeserializer.Deserialize(parser, expectedType, nestedObjectDeserializer, out value))
                {
                    if (value is null)
                        Logger.Error("Null value");
                    Validator.ValidateObject(value, new ValidationContext(value, null, null), true);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                value = null;
                return false;
            }
        }
    }
}
