using System;
using System.Collections.Generic;
using System.Text;
using OpenFAST.Template;
using OpenFAST.Template.operator_Renamed;
using OpenFAST;
using OpenFAST.Template.Type;

namespace UnitTest.Test
{
    public class ObjectMother
    {
        private static MessageTemplate _quoteTemplate;
        private static MessageTemplate _allocationInstruction;
        private static Group _instrument;
        private static Sequence _allocations;
        private static MessageTemplate _batchTemplate;
        private static MessageTemplate _headerTemplate;
        public static readonly int QUOTE_TEMPLATE_ID = 10;
        public static readonly int ALLOC_INSTRCTN_TEMPLATE_ID = 25;

        public static MessageTemplate QuoteTemplate()
        {
            if (_quoteTemplate == null)
            {
                _quoteTemplate = new MessageTemplate("Quote",
                        new Field[] {
                        new Scalar("bid", FASTType.DECIMAL, Operator.DELTA, ScalarValue.UNDEFINED, false),
                        new Scalar("ask", FASTType.DECIMAL, Operator.DELTA, ScalarValue.UNDEFINED, false)
                    });
            }

            return _quoteTemplate;
        }

        public static MessageTemplate BatchTemplate()
        {
            if (_batchTemplate == null)
            {
                _batchTemplate = new MessageTemplate("Batch",
                        new Field[] {
        				new StaticTemplateReference(HeaderTemplate()),
        				new Sequence("Batch", new Field[] {
        						DynamicTemplateReference.INSTANCE
        				}, false)
                    });
            }

            return _batchTemplate;
        }

        public static MessageTemplate HeaderTemplate()
        {
            if (_headerTemplate == null)
            {
                _headerTemplate = new MessageTemplate("Header",
                    new Field[] {
        			new Scalar("Sent", FASTType.U32, Operator.DELTA, ScalarValue.UNDEFINED, false)
                });
            }

            return _headerTemplate;
        }

        public static Message Quote(double bid, double ask)
        {
            Message quote = new Message(QuoteTemplate());
            quote.SetDecimal(1, bid);
            quote.SetDecimal(2, ask);

            return quote;
        }

        public static Message NewAllocInstrctn(String id, int side,
            double quantity, double averagePrice, GroupValue instrument,
            SequenceValue allocations)
        {
            Message allocInstrctn = new Message(AllocationInstruction());
            allocInstrctn.SetFieldValue(1, allocations);
            allocInstrctn.SetFieldValue(2, instrument);
            allocInstrctn.SetFieldValue(3, new StringValue(id));
            allocInstrctn.SetFieldValue(4, new IntegerValue(side));
            allocInstrctn.SetFieldValue(5, new DecimalValue(quantity));
            allocInstrctn.SetFieldValue(6, new DecimalValue(averagePrice));

            return allocInstrctn;
        }

        public static MessageTemplate AllocationInstruction()
        {
            if (_allocationInstruction == null)
            {
                _allocationInstruction = new MessageTemplate("AllocInstrctn",
                    new Field[] {
                    Allocations(), 
                    Instrument(),
                    new Scalar("ID",            FASTType.ASCII,   Operator.DELTA, ScalarValue.UNDEFINED, false),
                    new Scalar("Side",          FASTType.U32,     Operator.COPY,  ScalarValue.UNDEFINED, false),
                    new Scalar("Quantity",      FASTType.DECIMAL, Operator.DELTA, ScalarValue.UNDEFINED, false),
                    new Scalar("Average Price", FASTType.DECIMAL, Operator.DELTA, ScalarValue.UNDEFINED, false)
                });
            }

            return _allocationInstruction;
        }

        public static Sequence Allocations()
        {
            if (_allocations == null)
            {
                _allocations = new Sequence("Allocations",
                    new Field[] {
                    new Scalar("Account",       FASTType.ASCII,   Operator.COPY,  ScalarValue.UNDEFINED, false),
                    new Scalar("Price",         FASTType.DECIMAL, Operator.DELTA, ScalarValue.UNDEFINED, false),
                    new Scalar("Quantity",      FASTType.DECIMAL, Operator.DELTA, ScalarValue.UNDEFINED, false),
                    new Scalar("Average Price", FASTType.DECIMAL, Operator.DELTA, ScalarValue.UNDEFINED, false)
                }, false);
            }

            return _allocations;
        }

        private static Group Instrument()
        {
            if (_instrument == null)
            {
                _instrument = new Group("Instrmt",
                    new Field[] {
                    new Scalar("Symbol",        FASTType.ASCII,   Operator.COPY,  ScalarValue.UNDEFINED, false),
                    new Scalar("MMY",           FASTType.ASCII,   Operator.DELTA, ScalarValue.UNDEFINED, false),
                }, false);
            }

            return _instrument;
        }

        public static GroupValue NewInstrument(String symbol, String mmy)
        {
            return new GroupValue(Instrument(),
                new FieldValue[] { new StringValue(symbol), new StringValue(mmy) });
        }

        public static GroupValue NewAllocation(String account, double price,
            double quantity)
        {
            StringValue acct = account != null ? new StringValue(account) : null;
            return new GroupValue(Allocations().Group,
                new FieldValue[] {
                acct, new DecimalValue(price),
                new DecimalValue(quantity), new DecimalValue(0.0)
            });
        }

        public static Message BasicAllocationInstruction()
        {
            return NewAllocInstrctn("abcd1234", 2, 25.0, 102.0, BasicInstrument(), BasicAllocations());
        }

        private static SequenceValue BasicAllocations()
        {
            SequenceValue value = new SequenceValue(AllocationInstruction().GetSequence("Allocations"));
            value.Add(NewAllocation("general", 101.0, 15.0));
            value.Add(NewAllocation("specific", 103.0, 10.0));
            return value;
        }

        private static GroupValue BasicInstrument()
        {
            return NewInstrument("IBM", "200301");
        }
    }
}
