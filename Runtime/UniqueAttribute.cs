using System;
using UnityEngine;

namespace Zenvin.EditorUtil {
	/// <summary>
	/// Declares a serialized field as unique.<br></br>
	/// If the field is given a value that already exists on another object, the user will be prompted to enter a different value.
	/// <br></br>
	/// <b>Only works on <see cref="ScriptableObject"/> fields of types <see cref="int"/> and <see cref="string"/>!</b>
	/// </summary>
	/// <remarks>
	/// On some Unity versions, an <see cref="InvalidOperationException"/> may occur upon closing the prompt.<br></br>
	/// This will not affect the functinality of the attribute and can be ignored.
	/// </remarks>
	[AttributeUsage (AttributeTargets.Field)]
	public class UniqueAttribute : PropertyAttribute { }
}