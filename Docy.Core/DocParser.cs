using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using Jolt;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.IO;

namespace Docy.Core
{
	public class DocParser
	{
		XmlDocCommentReader _reader;

		public IEnumerable<Namespace> Parse(string assemblyFile)
		{
			return Parse(new string[] { assemblyFile });
		}

		public IEnumerable<Namespace> Parse(string[] assemblyFiles)
		{
			List<Namespace> namespaces = new List<Namespace>();

			foreach (string filename in assemblyFiles)
			{
				Assembly assembly = Assembly.LoadFrom(filename);
				_reader = new XmlDocCommentReader(filename.Replace(".dll", ".xml"));
				FindTypes(assembly, namespaces);

				foreach (Namespace nameSpace in namespaces)
				{
					nameSpace.Classes = nameSpace.Classes.OrderBy(c => c.Name).ToList();
				}
			}

			return namespaces;
		}

		private void FindTypes(Assembly assembly, List<Namespace> namespaces)
		{
			foreach (Type type in assembly.GetExportedTypes())
			{
				// The namespace is everything before this type name, e.g. [Docy.Core].XYZ
				string typeNamespace = type.Namespace;

				Namespace nameSpace = namespaces.FirstOrDefault(n => n.Name == typeNamespace);
				if (nameSpace == null)
				{
					nameSpace = new Namespace();
					nameSpace.Name = typeNamespace;
					namespaces.Add(nameSpace);
				}

				TypeBase typeBase = new TypeBase();
				typeBase.Namespace = nameSpace;
				typeBase.IsAbstract = type.IsAbstract;
				typeBase.IsNested = type.IsNested;
				typeBase.IsPrimitive = type.IsPrimitive;
				typeBase.IsPublic = type.IsPublic;
				typeBase.IsSealed = type.IsSealed;

				// Null for interfaces
				if (type.BaseType != null)
					typeBase.ParentClass = type.BaseType.FullName;
				else
					typeBase.ParentClass = "";

				typeBase.Parents = GetParents(type);

				typeBase.Name = GetTypeName(type);
				typeBase.Fullname = GetFullTypeName(type);
				typeBase.Constructors = GetConstructors(type,typeBase);
				typeBase.Methods = GetMethods(type, typeBase);
				typeBase.Properties = GetProperties(type, typeBase);

				// Comments
				XElement element = _reader.GetComments(type);
				CommentsBase comments = GetCommonTags(element);
				typeBase.Example = comments.Example;
				typeBase.Remarks = comments.Remarks;
				typeBase.Returns = comments.Returns;
				typeBase.Summary = comments.Summary;

				if (type.IsClass)
				{
					if (type.BaseType != null && type.BaseType == typeof(Delegate) || type.BaseType == typeof(MulticastDelegate))
					{
						typeBase.ObjectType = "Delegate";
						nameSpace.Delegates.Add(typeBase);
					}
					else
					{
						typeBase.ObjectType = "Class";
						nameSpace.Classes.Add(typeBase);
					}
				}
				else if (type.IsEnum)
				{
					typeBase.ObjectType = "Enumeration";
					typeBase.Members = GetMembers(type);
					nameSpace.Enumerations.Add(typeBase);
				}
				else if (type.IsValueType)
				{
					typeBase.ObjectType = "Structure";
					nameSpace.Structures.Add(typeBase);
				}
				else if (type.IsInterface)
				{
					typeBase.ObjectType = "Interface";
					nameSpace.Interfaces.Add(typeBase);
				}
				else 
				{
					typeBase.ObjectType = "Delegate";
					// TODO: Find out how to get delegate types
					nameSpace.Delegates.Add(typeBase);
				}
			}
		}

		private List<TypeSummary> GetParents(Type type)
		{
			List<TypeSummary> list = new List<TypeSummary>();

			Type current = type;

			while (current.BaseType != null)
			{
				TypeSummary summary = new TypeSummary();
				summary.Name = current.BaseType.Name;
				summary.Fullname = GetFullTypeName(current.BaseType);

				list.Add(summary);
				current = current.BaseType;
			}

			list.Reverse();
			return list;
		}

		private IList<Constructor> GetConstructors(Type type, TypeBase parent)
		{
			List<Constructor> list = new List<Constructor>();

			foreach (ConstructorInfo info in type.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
			{
				Constructor constructor = new Constructor();
				constructor.Parent = parent;
				constructor.Name = parent.Name;
				constructor.Fullname = info.ToString();
				constructor.Attributes = info.Attributes.ToString();
				constructor.Parent = parent;
				constructor.UseHashCodeForId = true;

				if (info.IsGenericMethod)
				{
					List<string> arguments = new List<string>();
					foreach (Type genericType in info.GetGenericArguments())
					{
						arguments.Add(genericType.Name);
					}

					constructor.Name = info.Name + "<" + string.Join(",", arguments) + ">";
					constructor.UseHashCodeForId = true;
				}

				// Get common tags
				XElement element = _reader.GetComments(info);
				CommentsBase comments = GetCommonTags(element);
				constructor.Example = comments.Example;
				constructor.Remarks = comments.Remarks;
				constructor.Returns = comments.Returns;
				constructor.Summary = comments.Summary;

				// Parameters
				constructor.Parameters = GetMethodParameters(info.GetParameters(), element);
				list.Add(constructor);
			}

			list = list.OrderBy(m => m.Name).ToList();
			return list;
		}

		private IList<Method> GetMethods(Type type, TypeBase parent)
		{
			List<Method> list = new List<Method>();

			// Used for ID generation
			List<string> methodNames = new List<string>();

			// Get only methods for this object, none of the inherited methods.
			foreach (MethodInfo info in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
			{
				if (!info.IsSpecialName)
				{
					Method method = new Method();
					method.Parent = parent;
					method.Name = info.Name;

					// Contains overloads - ID is the hashcode
					if (methodNames.Contains(info.Name))
						method.UseHashCodeForId = true;

					methodNames.Add(info.Name);

					if (info.IsGenericMethod)
					{
						List<string> arguments = new List<string>();
						foreach (Type genericType in info.GetGenericArguments())
						{
							arguments.Add(genericType.Name);
						}

						method.Name = info.Name + "<" + string.Join(",", arguments) + ">";
						method.UseHashCodeForId = true;
					}

					method.Fullname = info.ToString();
					method.ReturnType = GetTypeName(info.ReturnType);
					method.ReturnTypeFullName = GetFullTypeName(info.ReturnType);

					// Get common tags
					XElement element = null;
					if (info.IsGenericMethod)
						element = _reader.GetComments(info.GetGenericMethodDefinition());
					else
						element = _reader.GetComments(info);

					CommentsBase comments = GetCommonTags(element);
					method.Example = comments.Example;
					method.Remarks = comments.Remarks;
					method.Returns = comments.Returns;
					method.Summary = comments.Summary;

					// Parameters
					method.Parameters = GetMethodParameters(info.GetParameters(), element);

					list.Add(method);
				}
			}

			list = list.OrderBy(m => m.Name).ToList(); ;
			return list;
		}

		private IList<Parameter> GetMethodParameters(ParameterInfo[] parameters, XElement element)
		{
			IEnumerable<XElement> paramElements = new List<XElement>();

			if (element != null)
				paramElements = element.Elements().Where(e => e.Name.LocalName == "param");

			List<Parameter> list = new List<Parameter>();
			foreach (ParameterInfo info in parameters)
			{
				Parameter parameter = new Parameter();
				parameter.Name = info.Name;
				parameter.IsOut = info.IsOut;
				parameter.IsRet = info.IsRetval;
				parameter.Type = GetTypeName(info.ParameterType);
				parameter.TypeFullName = GetFullTypeName(info.ParameterType);

				XElement current = paramElements.FirstOrDefault(e => e.Attribute("name") != null && e.Attribute("name").Value == parameter.Name);
				if (current != null)
				{
					// TODO: get attributes
					parameter.Description = current.Value;

					if (!string.IsNullOrEmpty(parameter.Description))
						parameter.Description = parameter.Description.Trim();
				}

				list.Add(parameter);
			}

			return list;
		}

		private IList<Property> GetProperties(Type type, TypeBase parent)
		{
			List<Property> list = new List<Property>();

			// Get both public and protected properties, and no inherited ones unless overridden
			foreach (PropertyInfo info in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
			{
				Property property = new Property();
				property.Name = info.Name;
				property.Fullname = info.ToString();
				property.Attributes = info.Attributes.ToString();
				property.Parent = parent;
				property.Type = GetTypeName(info.PropertyType);
				property.TypeFullName = GetFullTypeName(info.PropertyType);

				XElement element = _reader.GetComments(info);
				CommentsBase comments = GetCommonTags(element);
				property.Example = comments.Example;
				property.Remarks = comments.Remarks;
				property.Returns = comments.Returns;
				property.Summary = comments.Summary;

				list.Add(property);
			}

			list = list.OrderBy(m => m.Name).ToList();
			return list;
		}

		private IList<MemberSummary> GetMembers(Type type)
		{
			// For enumerations
			List<MemberSummary> list = new List<MemberSummary>();

			foreach (MemberInfo info in type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
			{
				if (info.Name != "value__")
				{
					MemberSummary summary = new MemberSummary();
					summary.Name = info.Name;
					summary.Description = ""; // Needs to be added to Jolt

					XElement element = _reader.GetComments(info);
					CommentsBase comments = GetCommonTags(element);
					summary.Description = comments.Summary;

					list.Add(summary);
				}
			}

			list = list.OrderBy(m => m.Name).ToList();
			return list;
		}

		private CommentsBase GetCommonTags(XElement element)
		{
			CommentsBase comments = new CommentsBase();

			if (element != null)
			{
				// Example
				XElement current = element.Elements().FirstOrDefault(e => e.Name.LocalName == "example");
				if (current != null)
					comments.Example = current.Value;

				// Remarks
				current = element.Elements().FirstOrDefault(e => e.Name.LocalName == "remarks");
				if (current != null)
					comments.Remarks = current.Value;

				// Returns
				current = element.Elements().FirstOrDefault(e => e.Name.LocalName == "returns");
				if (current != null)
					comments.Returns = current.Value;

				// Summary
				current = element.Elements().FirstOrDefault(e => e.Name.LocalName == "summary");
				if (current != null)
					comments.Summary = current.Value;

				comments.Example = comments.Example.Trim();
				comments.Remarks = comments.Remarks.Trim();
				comments.Returns = comments.Returns.Trim();
				comments.Summary = comments.Summary.Trim();
			}

			return comments;
		}

		/// <summary>
		/// Get the type name with a sprinkle of magic dust if it's a generic type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private string GetTypeName(Type type)
		{
			string result = type.Name;

			if (type.IsGenericParameter || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)))
			{
				result = GetFriendlyGenericName(type);
			}

			return result;
		}

		private string GetFullTypeName(Type type)
		{
			string result = type.FullName;
			if (string.IsNullOrEmpty(result))
				result = type.Name;

			if (type.IsGenericParameter || (type.IsGenericType))
			{
				result = GetFriendlyGenericName(type);
			}

			return result;
		}

		private string GetFriendlyGenericName(Type paramType)
		{
			string result;

			if (paramType.IsGenericType || paramType.IsGenericParameter)
			{
				// Turn #1 into #3
				// #1 System.Linq.Expressions.Expression`1[[System.Func`2[[Roadkill.Core.User, Roadkill.Core, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null],[System.Object, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
				// #2 System.Linq.Expressions.Expression`1[[System.Func`2[[Roadkill.Core.User],[System.Object]]]]
				// #3 System.Linq.Expressions.Expression<Func<User,object>>

				CodeDomProvider csharpProvider = CodeDomProvider.CreateProvider("C#");
				CodeTypeReference typeReference = new CodeTypeReference(paramType);
				CodeVariableDeclarationStatement variableDeclaration = new CodeVariableDeclarationStatement(typeReference, "dummy");
				StringBuilder stringBuilder = new StringBuilder();
				using (StringWriter writer = new StringWriter(stringBuilder))
				{
					csharpProvider.GenerateCodeFromStatement(variableDeclaration, writer, new CodeGeneratorOptions());
				}

				stringBuilder.Replace(" dummy;", null);
				result = stringBuilder.ToString();
			}
			else
			{
				result = paramType.Name;
			}

			return result.Replace("\r", "").Replace("\n", "");
		}
	}
}
