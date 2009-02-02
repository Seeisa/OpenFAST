using System;
using BitVectorReader = OpenFAST.BitVectorReader;
using Context = OpenFAST.Context;
using FieldValue = OpenFAST.FieldValue;
using IntegerValue = OpenFAST.IntegerValue;
using Message = OpenFAST.Message;
using QName = OpenFAST.QName;
using ScalarValue = OpenFAST.ScalarValue;
using FastException = OpenFAST.Error.FastException;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template
{
	
	[Serializable]
	public sealed class MessageTemplate:Group, FieldSet
	{

        //public int FieldCount
        //{
        //    get
        //    {
        //        return fields.Length;
        //    }
			
        //}

		public new System.Type ValueType
		{
			get
			{
				return typeof(Message);
			}
			
		}

        //public Field[] Fields
        //{
        //    get
        //    {
        //        return fields;
        //    }
			
        //}

		public Field[] TemplateFields
		{
			get
			{
				Field[] f = new Field[fields.Length - 1];
				Array.Copy(fields, 1, f, 0, fields.Length - 1);
				return f;
			}
			
		}
		private const long serialVersionUID = 1L;
		
		public MessageTemplate(QName name, Field[] fields):base(name, AddTemplateIdField(fields), false)
		{
		}
		public override bool UsesPresenceMap()
		{
			return true;
		}
		public MessageTemplate(string name, Field[] fields):this(new QName(name), fields)
		{
		}

		private static Field[] AddTemplateIdField(Field[] fields)
		{
			Field[] newFields = new Field[fields.Length + 1];
			newFields[0] = new Scalar("templateId", FASTType.U32, Operator.COPY, ScalarValue.UNDEFINED, false);
			Array.Copy(fields, 0, newFields, 1, fields.Length);
			return newFields;
		}

		public override Field GetField(int index)
		{
			return fields[index];
		}

		public byte[] Encode(Message message, Context context)
		{
			if (!context.TemplateRegistry.IsRegistered(message.Template))
			{
				throw new FastException("Cannot encode message: The template " + message.Template + " has not been registered.", OpenFAST.Error.FastConstants.D9_TEMPLATE_NOT_REGISTERED);
			}
			message.SetInteger(0, context.GetTemplateId(message.Template));
			return base.Encode(message, this, context);
		}

		public Message Decode(System.IO.Stream in_Renamed, int templateId, BitVectorReader presenceMapReader, Context context)
		{
			try
			{
				if (context.TraceEnabled)
					context.DecodeTrace.GroupStart(this);
				FieldValue[] fieldValues = base.DecodeFieldValues(in_Renamed, this, presenceMapReader, context);
				fieldValues[0] = new IntegerValue(templateId);
				Message message = new Message(this, fieldValues);
				if (context.TraceEnabled)
					context.DecodeTrace.GroupEnd();
				return message;
			}
			catch (FastException e)
			{
				throw new FastException("An error occurred while decoding " + this, e.Code, e);
			}
		}
		public override string ToString()
		{
			return name.Name;
		}

		public override FieldValue CreateValue(string value_Renamed)
		{
			return new Message(this);
		}
		public  override bool Equals(System.Object obj)
		{
			if (obj == this)
				return true;
			if (obj == null || !(obj is MessageTemplate))
				return false;
			return Equals((MessageTemplate) obj);
		}

		public bool Equals(MessageTemplate other)
		{
			if (!name.Equals(other.name))
				return false;
			if (fields.Length != other.fields.Length)
				return false;
			for (int i = 0; i < fields.Length; i++)
			{
				if (!fields[i].Equals(other.fields[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode()
		{
			int hashCode = (name != null)?name.GetHashCode():0;
			for (int i = 0; i < fields.Length; i++)
				hashCode += fields[i].GetHashCode();
			return hashCode;
		}
	}
}