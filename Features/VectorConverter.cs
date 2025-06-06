using ProjectMER.Features.Extensions;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace ProjectMER.Features;

public class VectorConverter : IYamlTypeConverter
{
	/// <inheritdoc cref="IYamlTypeConverter" />
	public bool Accepts(Type type) => type == typeof(Vector3);

	/// <inheritdoc cref="IYamlTypeConverter" />
	public object ReadYaml(IParser parser, Type type)
	{
		string s = parser.Consume<Scalar>().Value;
		return s.ToVector3();
	}

	/// <inheritdoc cref="IYamlTypeConverter" />
	public void WriteYaml(IEmitter emitter, object? value, Type type) => emitter.Emit(new Scalar(((Vector3)value!).ToString("F3")));
}
