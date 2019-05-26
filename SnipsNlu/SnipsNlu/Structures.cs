using Newtonsoft.Json;
using System;
using System.Linq;

namespace SnipsNlu
{
    /// <summary>
    /// Results of the intent classifier
    /// </summary>
    public struct IntentClassifierResult
    {
        /// <summary>
        /// Name of the intent detected
        /// </summary>
        [JsonProperty("intentName")]
        public string IntentName { get; set; }

        /// <summary>
        /// Between 0 and 1
        /// </summary>
        [JsonProperty("confidenceScore")]
        public float ConfidenceScore { get; set; }

        public override string ToString()
        {
            return IntentName + " (" + ConfidenceScore.ToString("0.00%") + ")";
        }
    }

    /// <summary>
    /// Results of intent parsing
    /// </summary>
    public struct IntentParserResult
    {
        /// <summary>
        /// The text that was parsed
        /// </summary>
        [JsonProperty("input")]
        public string Input { get; set; }

        /// <summary>
        /// The result of intent classification, may be null if no intent was detected
        /// </summary>
        [JsonProperty("intent")]
        public IntentClassifierResult Intent { get; set; }

        /// <summary>
        /// The slots extracted if an intent was detected
        /// </summary>
        [JsonProperty("slots")]
        public Slot[] Slots { get; set; }

        public override string ToString()
        {
            return Input + "\n\t" + Intent.ToString() + string.Join("", Slots.Select(x => "\n\t\t" + x.ToString()));
        }
    }

    /// <summary>
    /// Struct describing a Slot
    /// </summary>
    public struct Slot
    {
        /// <summary>
        /// The resolved value of the slot
        /// </summary>
        [JsonProperty("value")]
        public SlotValue Value { get; set; }

        /// <summary>    
        /// The raw value as it appears in the input text
        /// </summary>
        [JsonProperty("rawValue")]
        public string RawValue { get; set; }

        /// <summary>
        /// Name of the entity type of the slot
        /// </summary>
        [JsonProperty("entity")]
        public string Entity { get; set; }

        /// <summary>
        /// Name of the slot
        /// </summary>
        [JsonProperty("slotName")]
        public string SlotName { get; set; }

        [JsonProperty("range")]
        public Range Range { get; set; }

        public override string ToString()
        {
            return "'" + Value.ToString() + "', '" + RawValue + "', " + Entity + ", " + SlotName + " @ " + Range;
        }
    }

    /// <summary>
    /// Struct describing a Slot
    /// </summary>
    public struct SlotValue
    {
        /// <summary>
        /// Points to either a * const char, a CNumberValue, a COrdinalValue,
        /// a CInstantTimeValue, a CTimeIntervalValue, a CAmountOfMoneyValue,
        /// a CTemperatureValue or a CDurationValue depending on value_type
        /// </summary>
        [JsonProperty("value")]
        public object Value { get; set; }

        /// <summary>
        /// The kind of the value
        /// </summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        /// <summary>
        /// The type of the value
        /// </summary>
        [JsonIgnore()]
        public SNIPS_SLOT_VALUE_TYPE ValueType
        {
            get
            {
                switch (Kind.ToUpper())
                {
                    case "AMOUNTOFMONEY":
                        return SNIPS_SLOT_VALUE_TYPE.SNIPS_SLOT_VALUE_TYPE_AMOUNTOFMONEY;
                    case "CUSTOM":
                        return SNIPS_SLOT_VALUE_TYPE.SNIPS_SLOT_VALUE_TYPE_CUSTOM;
                    case "DURATION":
                        return SNIPS_SLOT_VALUE_TYPE.SNIPS_SLOT_VALUE_TYPE_DURATION;
                    case "INSTANTTIME":
                        return SNIPS_SLOT_VALUE_TYPE.SNIPS_SLOT_VALUE_TYPE_INSTANTTIME;
                    case "MUSICALBUM":
                        return SNIPS_SLOT_VALUE_TYPE.SNIPS_SLOT_VALUE_TYPE_MUSICALBUM;
                    case "MUSICARTIST":
                        return SNIPS_SLOT_VALUE_TYPE.SNIPS_SLOT_VALUE_TYPE_MUSICARTIST;
                    case "MUSICTRACK":
                        return SNIPS_SLOT_VALUE_TYPE.SNIPS_SLOT_VALUE_TYPE_MUSICTRACK;
                    case "NUMBER":
                        return SNIPS_SLOT_VALUE_TYPE.SNIPS_SLOT_VALUE_TYPE_NUMBER;
                    case "ORDINAL":
                        return SNIPS_SLOT_VALUE_TYPE.SNIPS_SLOT_VALUE_TYPE_ORDINAL;
                    case "PERCENTAGE":
                        return SNIPS_SLOT_VALUE_TYPE.SNIPS_SLOT_VALUE_TYPE_PERCENTAGE;
                    case "TEMPERATURE":
                        return SNIPS_SLOT_VALUE_TYPE.SNIPS_SLOT_VALUE_TYPE_TEMPERATURE;
                    case "TIMEINTERVAL":
                        return SNIPS_SLOT_VALUE_TYPE.SNIPS_SLOT_VALUE_TYPE_TIMEINTERVAL;

                    default:
                        throw new NotImplementedException("SlotValue() : " + Kind);
                        //return 0;
                }
            }
        }

        public override string ToString()
        {
            return Value.ToString() + " (" + Kind + ")";
        }
    }

    public struct Range
    {
        /// <summary>
        /// Start index of raw value in input text
        /// </summary>
        [JsonProperty("start")]
        public string Start { get; set; }

        /// <summary>
        /// End index of raw value in input text
        /// </summary>
        [JsonProperty("end")]
        public string End { get; set; }

        public override string ToString()
        {
            return "[" + Start + ";" + End + "]";
        }
    }

    /// <summary>
    /// Enum type describing how to cast the value of a CSlotValue
    /// </summary>
    public enum SNIPS_SLOT_VALUE_TYPE
    {
        /// <summary>
        /// Custom type represented by a char * 
        /// </summary>
        SNIPS_SLOT_VALUE_TYPE_CUSTOM = 1,

        /// <summary>
        /// Number type represented by a CNumberValue
        /// </summary>
        SNIPS_SLOT_VALUE_TYPE_NUMBER = 2,

        /// <summary>
        /// Ordinal type represented by a COrdinalValue
        /// </summary>
        SNIPS_SLOT_VALUE_TYPE_ORDINAL = 3,

        /// <summary>
        /// Instant type represented by a CInstantTimeValue
        /// </summary>
        SNIPS_SLOT_VALUE_TYPE_INSTANTTIME = 4,

        /// <summary>
        /// Interval type represented by a CTimeIntervalValue
        /// </summary>
        SNIPS_SLOT_VALUE_TYPE_TIMEINTERVAL = 5,

        /// <summary>
        /// Amount of money type represented by a CAmountOfMoneyValue
        /// </summary>
        SNIPS_SLOT_VALUE_TYPE_AMOUNTOFMONEY = 6,

        /// <summary>
        /// Temperature type represented by a CTemperatureValue
        /// </summary>
        SNIPS_SLOT_VALUE_TYPE_TEMPERATURE = 7,

        /// <summary>
        /// Duration type represented by a CDurationValue
        /// </summary>
        SNIPS_SLOT_VALUE_TYPE_DURATION = 8,

        /// <summary>
        /// Percentage type represented by a CPercentageValue
        /// </summary>
        SNIPS_SLOT_VALUE_TYPE_PERCENTAGE = 9,

        /// <summary>
        /// Music Album type represented by a char * 
        /// </summary>
        SNIPS_SLOT_VALUE_TYPE_MUSICALBUM = 10,

        /// <summary>
        /// Music Artist type represented by a char * 
        /// </summary>
        SNIPS_SLOT_VALUE_TYPE_MUSICARTIST = 11,

        /// <summary>
        /// Music Track type represented by a char *
        /// </summary>
        SNIPS_SLOT_VALUE_TYPE_MUSICTRACK = 12,
    }
}
