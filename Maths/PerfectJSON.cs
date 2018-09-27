using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using System.IO;
using System.Text;

namespace Maths
{

    /// <summary>
    /// Attribut servant à déterminer si une propriété doit être sérialisée ou non
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PerfectSerializableAttribute : Attribute
    {
        public readonly bool _serializable;

        public PerfectSerializableAttribute(bool serializable)
        {
            _serializable = serializable;
        }
    }

    /// <summary>
    /// Options pour sérialiser ou désérialiser des variables
    /// </summary>
    public enum JsonOptions
    {
        /// <summary>
        /// Sans options
        /// </summary>
        DEFAULT = 0,
        /// <summary>
        /// Tous les caractères &lt; et &gt; sont convertis en séquences \u003C et \u003E
        /// </summary>
        HEX_TAG = 1,
        /// <summary>
        /// Tous les caractères &amp; sont convertis en \u0026
        /// </summary>
        HEX_AMP = 2,
        /// <summary>
        /// Tous les guillemets ' sont convertis en \u0027
        /// </summary>
        HEX_APOS = 4,
        /// <summary>
        /// Tous les guillemets doubles " sont convertis en \u0022
        /// </summary>
        HEX_QUOT = 8,
        /// <summary>
        /// Produit un objet plutôt qu'un tableau, lorsqu'un tableau non-associatif est utilisé
        /// </summary>
        FORCE_OBJECT = 16,
        /// <summary>
        /// Encode les chaînes numériques en tant que nombres (inutilisé)
        /// </summary>
        NUMERIC_CHECK = 32,
        /// <summary>
        /// Encode les gros entiers sous forme d'une chaîne de caractères (inutilisé)
        /// </summary>
        BIGINT_AS_STRING = 64,
        /// <summary>
        /// Ne pas échapper les caractères /
        /// </summary>
        UNESCAPED_SLASHES = 128,
        /// <summary>
        /// Utilise des espaces dans les données retournées pour les formater
        /// </summary>
        PRETTY_PRINT = 256,
        /// <summary>
        /// Encode les caractères multi-octets Unicode littéralement (inutilisé)
        /// </summary>
        UNESCAPED_UNICODE = 512,
        /// <summary>
        /// S'assure que les valeurs de type float sont toujours encodées comme valeurs de type float (inutilisé)
        /// </summary>
        PRESERVE_ZERO_FRACTION = 1024
    }

    /// <summary>
    /// Classe utilisée pour sérialiser / désérialiser les champs et propriétés d'un objet au format JSON
    /// </summary>
    public static class PerfectJSON
    {

        /// <summary>
        /// Sérialise les champs et les propriétés d'un objet et les retourne sous format JSON
        /// </summary>
        /// <param name="obj">Objet à sérialiser</param>
        /// <param name="options">Options de sérialisation</param>
        /// <returns>Chaîne de caractères au format JSON</returns>
        public static string Serialize(object obj, JsonOptions options = JsonOptions.DEFAULT)
        {
            return Serializer.Serialize(obj, options);
        }

        /// <summary>
        /// Désérialise une chaîne de caractères au format JSON et retourne un objet avec le type prédéfini
        /// </summary>
        /// <typeparam name="T">Type prédéfini</typeparam>
        /// <param name="json">Chaîne de caractères au format JSON</param>
        /// <param name="options">Options de désérialisation</param>
        /// <returns>Objet de type prédéfini</returns>
        public static T Deserialize<T>(string json, JsonOptions options = JsonOptions.DEFAULT)
        {
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                if (type.IsDictionary())
                {
                    return (T)Deserializer.DeserializeDictionary(json, type, options);
                }
                else
                {
                    return (T)Deserializer.DeserializeList(json, type, options);
                }
            }

            T obj = Activator.CreateInstance<T>();
            Deserializer.Deserialize(json, obj, options);
            return obj;
        }

        /// <summary>
        /// Désérialise une chaîne de caractères au format JSON et retourne un objet avec le type déterminé
        /// </summary>
        /// <param name="json">Chaîne de caractères au format JSON</param>
        /// <param name="type">Type déterminé</param>
        /// <param name="options">Options de désérialisation</param>
        /// <returns>Objet de type déterminé</returns>
        public static object Deserialize(string json, Type type, JsonOptions options = JsonOptions.DEFAULT)
        {
            if (type.IsGenericType)
            {
                if (type.IsDictionary())
                {
                    return Deserializer.DeserializeDictionary(json, type, options);
                }
                else
                {
                    return Deserializer.DeserializeList(json, type, options);
                }
            }

            object obj = Activator.CreateInstance(type);
            Deserializer.Deserialize(json, obj, options);
            return obj;
        }

        /// <summary>
        /// Désérialise une chaîne de caractères au format JSON et redéfinit les champs et propriétés d'un objet
        /// </summary>
        /// <param name="json">Chaîne de caractères au format JSON</param>
        /// <param name="obj">Objet à redéfinir</param>
        /// <param name="options">Options de désérialisation</param>
        public static void DeserializeOverwrite(string json, object obj, JsonOptions options = JsonOptions.DEFAULT)
        {
            Type type = obj.GetType();
            if (type.IsGenericType)
            {
                if (type.IsDictionary())
                {
                    obj = Deserializer.DeserializeDictionary(json, type, options);
                }
                else
                {
                    obj = Deserializer.DeserializeList(json, type, options);
                }
            }
            else
            {
                Deserializer.Deserialize(json, obj, options);
            }
        }

        /// <summary>
        /// Vérifie si un type donné est un dictionnaire
        /// </summary>
        /// <param name="type">Type donné</param>
        /// <returns>Le type est-il un dictionnaire</returns>
        public static bool IsDictionary(this Type type)
        {
            return type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        #region SerialManager

        /// <summary>
        /// Classe servant à stocker les partages communs entre le sérialiseur et le déserialiseur
        /// </summary>
        private abstract class SerialManager
        {         
            protected JsonOptions options;
            protected FieldInfo[] fields;
            protected PropertyInfo[] properties;

            protected SerialManager(Type type, JsonOptions op, bool get)
            {
                options = op;
                BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
                fields = type.GetFields(flags).Where(f => f.FieldType != type).ToArray();
                properties = type.GetProperties(flags).Where(p => ValidProperty(p, type, get)).ToArray();
            }

            private bool ValidProperty(PropertyInfo prop, Type type, bool get)
            {
                if (prop.GetIndexParameters().Length > 0 || prop.PropertyType == type)
                {
                    return false;
                }

                if (get)
                {
                    if (!prop.CanRead)
                    {
                        return false;
                    }

                    foreach (object attr in prop.GetCustomAttributes(false))
                    {
                        if (attr is PerfectSerializableAttribute && !(attr as PerfectSerializableAttribute)._serializable)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return prop.CanWrite;
            }

            protected bool HasOptions(JsonOptions op)
            {
                return (options & op) == op;
            }
        }
        #endregion

        #region Serialization

        /// <summary>
        /// Classe servant à sérialiser un objet en chaîne de caractères au format JSON
        /// </summary>
        private sealed class Serializer : SerialManager
        {
            private StringBuilder json;
            private bool prettyPrint;
            private int indentation;

            private Serializer(object obj, JsonOptions op, int indent) : base(obj.GetType(), op, true)
            {
                json = new StringBuilder();
                prettyPrint = HasOptions(JsonOptions.PRETTY_PRINT);
                indentation = indent;

                if (obj is IList)
                {
                    SerializeList((IList)obj);
                    return;
                }

                if (obj is IDictionary)
                {
                    SerializeDictionary((IDictionary)obj);
                    return;
                }

                json.Append('{');
                JumpLine(1);

                bool first = true;
                foreach (FieldInfo field in fields)
                {
                    if (!first)
                    {
                        json.Append(',');
                        JumpLine();
                    }
                    SerializeField(field, obj);
                    first = false;
                }

                foreach (PropertyInfo property in properties)
                {
                    if (!first)
                    {
                        json.Append(',');
                        JumpLine();
                    }
                    SerializeProperty(property, obj);
                    first = false;
                }

                JumpLine(-1);
                json.Append('}');
            }

            public static string Serialize(object obj, JsonOptions op, int indent = 0)
            {
                Serializer instance = new Serializer(obj, op, indent);

                return instance.json.ToString();
            }

            private void JumpLine(int addIndent = 0)
            {
                if (prettyPrint)
                {
                    json.Append('\n');
                    indentation += addIndent;
                    json.Append(' ', 4 * indentation);
                }
            }

            private void SerializeMemberName(MemberInfo member)
            {
                SerializeString(member.Name);
                json.Append(':');
                if (prettyPrint)
                {
                    json.Append(' ');
                }
            }

            private void SerializeField(FieldInfo field, object obj)
            {
                SerializeMemberName(field);

                object value = field.GetValue(obj);
                SerializeValue(value);
            }

            private void SerializeProperty(PropertyInfo property, object obj)
            {
                SerializeMemberName(property);

                object value = property.GetValue(obj, null);
                SerializeValue(value);
            }

            private void SerializeValue(object value)
            {
                string asStr;
                IList asList;
                IDictionary asDic;

                if (value == null)
                {
                    json.Append("\"null\"");
                }
                else if ((asStr = value as string) != null)
                {
                    SerializeString(asStr);
                }
                else if (value is char)
                {
                    SerializeString(value.ToString());
                }
                else if ((asList = value as IList) != null)
                {
                    SerializeList(asList);
                }
                else if ((asDic = value as IDictionary) != null)
                {
                    SerializeDictionary(asDic);
                }
                else
                {
                    SerializeOther(value);
                }
            }

            private void SerializeString(string str)
            {
                bool unescaped = HasOptions(JsonOptions.UNESCAPED_SLASHES);

                List<char> conditions = new List<char>();
                if (HasOptions(JsonOptions.HEX_TAG))
                {
                    conditions.Add('<');
                    conditions.Add('>');
                }
                if (HasOptions(JsonOptions.HEX_AMP))
                {
                    conditions.Add('&');
                }
                if (HasOptions(JsonOptions.HEX_APOS))
                {
                    conditions.Add('\'');
                }
                if (HasOptions(JsonOptions.HEX_QUOT))
                {
                    conditions.Add('"');
                }


                json.Append('"');

                foreach (char c in str)
                {
                    switch (c)
                    {
                        case '/':
                            if (unescaped)
                            {
                                json.Append(c);
                            }
                            else
                            {
                                json.Append("\\/");
                            }
                            break;

                        case '\\':
                            json.Append("\\\\");
                            break;

                        case '\b':
                            json.Append("\\b");
                            break;

                        case '\f':
                            json.Append("\\f");
                            break;

                        case '\n':
                            json.Append("\\n");
                            break;

                        case '\r':
                            json.Append("\\r");
                            break;

                        case '\t':
                            json.Append("\\t");
                            break;

                        default:
                            if (c < 32 || c > 126 || conditions.Contains(c))
                            {
                                json.Append("\\u" + Convert.ToString(c, 16).PadLeft(4, '0'));
                            }
                            else
                            {
                                if (c == '"')
                                {
                                    json.Append("\\\"");
                                }
                                else
                                {
                                    json.Append(c);
                                }
                            }
                            break;
                    }
                }

                json.Append('"');
            }

            private void SerializeList(IList list)
            {
                bool forceObject = HasOptions(JsonOptions.FORCE_OBJECT);

                json.Append(forceObject ? '{' : '[');
                JumpLine(1);

                bool first = true;

                int i = 0;
                foreach (object obj in list)
                {
                    if (!first)
                    {
                        json.Append(',');
                        JumpLine();
                    }

                    if (forceObject)
                    {
                        json.Append('"');
                        json.Append(i);
                        json.Append("\":");
                        if (prettyPrint)
                        {
                            json.Append(' ');
                        }
                        ++i;
                    }

                    SerializeValue(obj);

                    first = false;
                }

                JumpLine(-1);
                json.Append(forceObject ? '}' : ']');
            }

            private void SerializeDictionary(IDictionary dic)
            {
                json.Append('{');
                JumpLine(1);

                bool first = true;

                foreach (object obj in dic.Keys)
                {
                    if (!first)
                    {
                        json.Append(',');
                        JumpLine();
                    }

                    SerializeValue(obj);
                    json.Append(':');
                    if (prettyPrint)
                    {
                        json.Append(' ');
                    }
                    SerializeValue(dic[obj]);

                    first = false;
                }

                JumpLine(-1);
                json.Append('}');
            }

            private void SerializeOther(object value)
            {
                if (value is float
                    || value is int
                    || value is uint
                    || value is long
                    || value is double
                    || value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is ulong
                    || value is decimal)
                {
                    json.Append(value.ToString());
                }
                else if (value is Enum)
                {
                    json.Append((int)value);
                }
                else
                {
                    json.Append(Serialize(value, options, indentation));
                }
            }
        }
        #endregion

        #region Deserialization

        /// <summary>
        /// Classe servant à désérialiser une chaîne de caractères au format JSON en objet
        /// </summary>
        private sealed class Deserializer : SerialManager, IDisposable
        {
            private const string WHITE_SPACE = " \t\n\r";
            private const string WORD_BREAK = " \t\n\r{}[],:\"";

            private enum TOKEN
            {
                NONE,
                CURLY_OPEN,
                CURLY_CLOSE,
                SQUARED_OPEN,
                SQUARED_CLOSE,
                COLON,
                COMMA,
                STRING,
                NUMBER,
                TRUE,
                FALSE,
                NULL
            }

            private object obj;
            private StringReader reader;

            private char PeekChar
            {
                get
                {
                    return (char)reader.Peek();
                }
            }

            private char NextChar
            {
                get
                {
                    return (char)reader.Read();
                }
            }

            private string NextWord
            {
                get
                {
                    StringBuilder word = new StringBuilder();

                    while (WORD_BREAK.IndexOf(PeekChar) == -1)
                    {
                        word.Append(NextChar);

                        if (reader.Peek() == -1)
                        {
                            break;
                        }
                    }

                    return word.ToString();
                }
            }

            private TOKEN NextToken
            {
                get
                {
                    EatWhiteSpace();

                    if (reader.Peek() == -1)
                    {
                        return TOKEN.NONE;
                    }

                    char c = PeekChar;
                    switch (c)
                    {
                        case '{':
                            return TOKEN.CURLY_OPEN;

                        case '}':
                            reader.Read();
                            return TOKEN.CURLY_CLOSE;

                        case '[':
                            return TOKEN.SQUARED_OPEN;

                        case ']':
                            reader.Read();
                            return TOKEN.SQUARED_CLOSE;

                        case ',':
                            reader.Read();
                            return TOKEN.COMMA;

                        case '"':
                            return TOKEN.STRING;

                        case ':':
                            return TOKEN.COLON;

                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case '-':
                            return TOKEN.NUMBER;
                    }

                    string word = NextWord;
                    switch (word)
                    {
                        case "false":
                            return TOKEN.FALSE;

                        case "true":
                            return TOKEN.TRUE;

                        case "null":
                            return TOKEN.NULL;
                    }

                    return TOKEN.NONE;
                }
            }

            private Deserializer(string json, object o, JsonOptions op) : base(o.GetType(), op, false)
            {
                obj = o;
                reader = new StringReader(json);
            }

            public static void Deserialize(string json, object o, JsonOptions op)
            {
                if (string.IsNullOrEmpty(json))
                {
                    o = null;
                    return;
                }

                using (Deserializer deserializer = new Deserializer(json, o, op))
                {
                    deserializer.DeserializeObject();
                }
            }

            public static IList DeserializeList(string json, Type listType, JsonOptions op)
            {
                if (string.IsNullOrEmpty(json))
                {
                    return null;
                }

                using (Deserializer deserializer = new Deserializer(json, null, op))
                {
                    return deserializer.DeserializeList(listType);
                }
            }

            public static IDictionary DeserializeDictionary(string json, Type dicoType, JsonOptions op)
            {
                if (string.IsNullOrEmpty(json))
                {
                    return null;
                }

                using (Deserializer deserializer = new Deserializer(json, null, op))
                {
                    return deserializer.DeserializeDictionary(dicoType);
                }
            }

            public void Dispose()
            {
                reader.Dispose();
                reader = null;
            }

            private void EatWhiteSpace()
            {
                while (WHITE_SPACE.IndexOf(PeekChar) != -1)
                {
                    reader.Read();

                    if (reader.Peek() == -1)
                    {
                        break;
                    }
                }
            }

            private void DeserializeObject()
            {
                if (NextToken != TOKEN.CURLY_OPEN)
                {
                    throw new FormatException();
                }

                reader.Read();

                while (true)
                {
                    switch (NextToken)
                    {
                        case TOKEN.NONE:
                            return;

                        case TOKEN.COMMA:
                            continue;

                        case TOKEN.CURLY_CLOSE:
                            return;

                        default:
                            string name = DeserializeString();

                            if (name == null || name == "")
                            {
                                throw new FormatException();
                            }


                            if (fields.Any(f => f.Name == name))
                            {
                                FieldInfo field = fields.First(f => f.Name == name);

                                if (NextToken != TOKEN.COLON)
                                {
                                    throw new FormatException();
                                }

                                reader.Read();

                                object value = DeserializeValue(field.FieldType);

                                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                                {
                                    object list = field.GetValue(obj);

                                    if (list == null)
                                    {
                                        field.SetValue(obj, value);
                                    }
                                    else
                                    {
                                        IList iList = (IList)list;
                                        iList.Clear();
                                        foreach (object element in (IList)value)
                                        {
                                            iList.Add(element);
                                        }
                                    }
                                }
                                else
                                {
                                    field.SetValue(obj, value);
                                }
                            }
                            else if (properties.Any(p => p.Name == name))
                            {
                                PropertyInfo property = properties.First(p => p.Name == name);

                                if (NextToken != TOKEN.COLON)
                                {
                                    throw new FormatException();
                                }

                                reader.Read();

                                object value = DeserializeValue(property.PropertyType);
                                property.SetValue(obj, value, null);
                            }
                            else
                            {
                                bool found = false;
                                bool quote = false;
                                int braces = 0;

                                while (!found && braces >= 0)
                                {
                                    char c = NextChar;
                                    switch (c)
                                    {
                                        case '"':
                                            quote = !quote;
                                            break;

                                        case '[':
                                        case '{':
                                            if (!quote)
                                            {
                                                ++braces;
                                            }
                                            break;

                                        case ']':
                                        case '}':
                                            if (!quote)
                                            {
                                                --braces;
                                            }
                                            break;

                                        case ',':
                                            if (!quote && braces == 0)
                                            {
                                                found = true;
                                            }
                                            break;
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            private object DeserializeValue(Type type)
            {
                return DeserializeByToken(NextToken, type);
            }

            private object DeserializeByToken(TOKEN token, Type type)
            {
                switch (token)
                {
                    case TOKEN.STRING:
                        string value = DeserializeString();

                        if (type == typeof(char))
                        {
                            return value[0];
                        }
                        else
                        {
                            return value;
                        }

                    case TOKEN.NUMBER:
                        return DeserializeNumber(type);

                    case TOKEN.SQUARED_OPEN:
                        return DeserializeList(type);

                    case TOKEN.CURLY_OPEN:
                        return DeserializeSubObject(type);

                    case TOKEN.TRUE:
                        return true;

                    case TOKEN.FALSE:
                        return false;

                    default:
                        return null;
                }
            }

            private object DeserializeSubObject(Type type)
            {
                if (type.IsGenericType)
                {
                    if (type.IsDictionary())
                    {
                        return DeserializeDictionary(type);
                    }
                    else if (HasOptions(JsonOptions.FORCE_OBJECT))
                    {
                        return DeserializeList(type);
                    }
                    else
                    {
                        throw new FormatException();
                    }
                }

                StringBuilder subObject = new StringBuilder();

                int braces = 0;
                do
                {
                    char c = NextChar;
                    if (c == '{')
                    {
                        ++braces;
                    }
                    else if (c == '}')
                    {
                        --braces;
                    }
                    subObject.Append(c);
                }
                while (braces > 0);

                return PerfectJSON.Deserialize(subObject.ToString(), type);
            }

            private string DeserializeString()
            {
                StringBuilder s = new StringBuilder();
                char c;

                reader.Read();

                bool parsing = true;
                while (parsing)
                {
                    if (reader.Peek() == -1)
                    {
                        parsing = false;
                        break;
                    }

                    c = NextChar;
                    switch (c)
                    {
                        case '"':
                            parsing = false;
                            break;

                        case '\\':
                            if (reader.Peek() == -1)
                            {
                                parsing = false;
                                break;
                            }

                            c = NextChar;
                            switch (c)
                            {
                                case '"':
                                case '\\':
                                case '/':
                                    s.Append(c);
                                    break;

                                case 'b':
                                    s.Append('\b');
                                    break;

                                case 'f':
                                    s.Append('\f');
                                    break;

                                case 'n':
                                    s.Append('\n');
                                    break;

                                case 'r':
                                    s.Append('\r');
                                    break;

                                case 't':
                                    s.Append('\t');
                                    break;

                                case 'u':
                                    StringBuilder hex = new StringBuilder();

                                    for (int i = 0; i < 4; ++i)
                                    {
                                        hex.Append(NextChar);
                                    }

                                    s.Append((char)Convert.ToInt32(hex.ToString(), 16));
                                    break;
                            }
                            break;

                        default:
                            s.Append(c);
                            break;
                    }
                }
                return s.ToString();
            }

            private IDictionary DeserializeDictionary(Type dicType)
            {
                object instance = Activator.CreateInstance(dicType);

                IDictionary table = (IDictionary)instance;

                Type[] arguments = dicType.GetGenericArguments();

                reader.Read();

                while (true)
                {
                    switch (NextToken)
                    {
                        case TOKEN.NONE:
                            return null;

                        case TOKEN.COMMA:
                            continue;

                        case TOKEN.CURLY_CLOSE:
                            return table;

                        default:

                            object key = DeserializeValue(arguments[0]);

                            if (NextToken != TOKEN.COLON)
                            {
                                return null;
                            }

                            reader.Read();

                            table[key] = DeserializeValue(arguments[1]);

                            break;
                    }
                }
            }

            private IList DeserializeList(Type listType)
            {
                bool forceObject = HasOptions(JsonOptions.FORCE_OBJECT);

                object instance;
                Type argument;

                bool generic = listType.IsGenericType;

                if (generic)
                {
                    instance = Activator.CreateInstance(listType);
                    argument = listType.GetGenericArguments()[0];
                }
                else
                {
                    instance = new List<object>();
                    argument = listType.GetElementType();
                }

                IList list = (IList)instance;

                reader.Read();

                bool parsing = true;

                while (parsing)
                {
                    TOKEN nextToken = NextToken;

                    switch (nextToken)
                    {
                        case TOKEN.NONE:
                            return null;

                        case TOKEN.COMMA:
                            continue;

                        case TOKEN.SQUARED_CLOSE:
                            if (!forceObject)
                            {
                                parsing = false;
                            }
                            break;

                        case TOKEN.CURLY_CLOSE:
                            if (forceObject)
                            {
                                parsing = false;
                            }
                            break;

                        default:
                            if (forceObject && nextToken == TOKEN.STRING)
                            {
                                DeserializeString();
                                if (NextToken != TOKEN.COLON)
                                {
                                    return null;
                                }
                                reader.Read();
                            }

                            object value = DeserializeByToken(NextToken, argument);

                            list.Add(value);
                            break;
                    }
                }

                if (generic)
                {
                    return list;
                }

                Array array = Array.CreateInstance(argument, list.Count);

                for (int i = 0; i < list.Count; ++i)
                {
                    array.SetValue(list[i], i);
                }

                return array;
            }

            private object DeserializeNumber(Type type)
            {
                string number = NextWord;

                if (type.GetMethods().Any(m => m.Name == "TryParse"))
                {
                    object value = Activator.CreateInstance(type);
                    object[] parameters = new object[] { number, value };

                    MethodInfo parse = type.GetMethod("TryParse", new Type[] { typeof(string), type.MakeByRefType() });

                    object result = parse.Invoke(null, parameters);

                    if ((bool)result)
                    {
                        value = parameters[1];
                    }

                    return value;
                }
                else if (type.IsEnum)
                {
                    int value;
                    int.TryParse(number, out value);
                    return value;
                }
                else
                {
                    double value;
                    double.TryParse(number, out value);
                    return value;
                }
            }
        }
        #endregion
    }
}