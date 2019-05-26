using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SnipsNlu
{
    /// <summary>
    /// 
    /// </summary>
    public class SnipsNLUEngine : IDisposable
    {
        public const string SnipsNluVersion = "0.64.3";

        private IntPtr _client { get; set; }

        /// <summary>
        /// Loads an NluEngine from a zip file
        /// </summary>
        /// <param name="rootDir"></param>
        public SnipsNLUEngine(byte[] rootDir, uint zipSize)
        {
            var tempPrt = IntPtr.Zero;
            if (NativeMethods.snips_nlu_engine_create_from_zip(rootDir, zipSize, ref tempPrt) != SNIPS_RESULT.SNIPS_RESULT_OK)
            {
                this._client = IntPtr.Zero;
                throw NluEngineError.GetLast();
            }
            _client = tempPrt;
        }

        /// <summary>
        /// Create an NluEngine from a zip file
        /// </summary>
        /// <param name="rootZip"></param>
        /// <returns></returns>
        public static SnipsNLUEngine CreateFromZip(string rootZip)
        {
            var bytes = File.ReadAllBytes(rootZip);
            return new SnipsNLUEngine(bytes, (uint)bytes.Length);
        }

        /// <summary>
        /// Loads an NluEngine from a directory
        /// </summary>
        /// <param name="rootDir"></param>
        public SnipsNLUEngine(string rootDir)
        {
            var tempPrt = IntPtr.Zero;
            if (NativeMethods.snips_nlu_engine_create_from_dir(rootDir, ref tempPrt) != SNIPS_RESULT.SNIPS_RESULT_OK)
            {
                this._client = IntPtr.Zero;
                throw NluEngineError.GetLast();
            }
            _client = tempPrt;
        }

        /// <summary>
        /// Create an NluEngine from a directory
        /// </summary>
        /// <param name="rootDir"></param>
        /// <returns></returns>
        public static SnipsNLUEngine CreateFromDirectory(string rootDir)
        {
            return new SnipsNLUEngine(rootDir);
        }

        public void Dispose()
        {
            if (NativeMethods.snips_nlu_engine_destroy_client(_client) != SNIPS_RESULT.SNIPS_RESULT_OK)
            {
                throw NluEngineError.GetLast();
            }
            _client = IntPtr.Zero;
        }

        /// <summary>
        /// Extracts intent and slots from the input
        /// </summary>
        /// <param name="str">input to process</param>
        /// <param name="intentsWhitelist">[TODO] optional list of intents used to restrict the parsing scope</param>
        /// <param name="intentsBlacklist">[TODO] optional list of intents to exclude during parsing</param>
        public IntentParserResult Parse(string str, string[] intentsWhitelist = null, string[] intentsBlacklist = null)
        {
            if (intentsWhitelist != null || intentsBlacklist != null)
            {
                throw new NotImplementedException();
            }

            string json = "";
            if (NativeMethods.snips_nlu_engine_run_parse_into_json(_client, EncodeUTF8(str), null, null, ref json) != SNIPS_RESULT.SNIPS_RESULT_OK)
            {
                throw NluEngineError.GetLast();
            }
            return JsonConvert.DeserializeObject<IntentParserResult>(json);
        }

        /// <summary>
        /// Extracts slots from the input when the intent is known.
        /// </summary>
        /// <param name="str">input to process</param>
        /// <param name="intent">intent which the input corresponds to</param>
        public Slot[] GetSlots(string str, string intent)
        {
            string json = "";
            if (NativeMethods.snips_nlu_engine_run_get_slots_into_json(_client, EncodeUTF8(str), EncodeUTF8(intent), ref json) != SNIPS_RESULT.SNIPS_RESULT_OK)
            {
                throw NluEngineError.GetLast();
            }
            return JsonConvert.DeserializeObject<Slot[]>(json);
        }

        /// <summary>
        /// Extracts the list of intents ranked by their confidence score.
        /// </summary>
        /// <param name="str"></param>
        public IntentClassifierResult[] GetIntents(string str)
        {
            string json = "";
            if (NativeMethods.snips_nlu_engine_run_get_intents_into_json(_client, EncodeUTF8(str), ref json) != SNIPS_RESULT.SNIPS_RESULT_OK)
            {
                throw NluEngineError.GetLast();
            }
            return JsonConvert.DeserializeObject<IntentClassifierResult[]>(json);
        }

        public static string GetModelVersion()
        {
            IntPtr versionPtr = IntPtr.Zero;
            if (NativeMethods.snips_nlu_engine_get_model_version(ref versionPtr) != SNIPS_RESULT.SNIPS_RESULT_OK)
            {
                throw NluEngineError.GetLast();
            }
            return Marshal.PtrToStringAnsi(versionPtr);
        }

        public static bool Is64bit
        {
            get
            {
                return NativeMethods.Is64bit;
            }
        }

        private string EncodeUTF8(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(input));
        }

        /*
         * https://github.com/snipsco/snips-nlu-rs/blob/develop/platforms/c/libsnips_nlu.h
         * https://github.com/snipsco/snips-nlu-rs/blob/develop/platforms/swift/SnipsNlu/SnipsNlu/NluEngine.swift
         */
        internal static class NativeMethods
        {
            internal static bool Is64bit => IntPtr.Size == 8;
            private const string DLL = "snips_nlu_ffi.dll";

            static NativeMethods()
            {
                var subfolder = Is64bit ? "x64" : "x86";
                LoadLibrary(subfolder + @"\" + DLL);
            }

            [DllImport("kernel32.dll")]
            private static extern IntPtr LoadLibrary(string dllToLoad);

            [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            internal static extern SNIPS_RESULT snips_nlu_engine_create_from_dir(string root_dir, [In, Out] ref IntPtr client);

            [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            internal static extern SNIPS_RESULT snips_nlu_engine_create_from_zip(byte[] zip, uint zip_size, [In, Out] ref IntPtr client);

            [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            internal static extern SNIPS_RESULT snips_nlu_engine_destroy_client(IntPtr client);

            // SNIPS_RESULT snips_nlu_engine_destroy_intent_classifier_results(CIntentClassifierResultList* result);
            // SNIPS_RESULT snips_nlu_engine_destroy_result(CIntentParserResult *result);
            // SNIPS_RESULT snips_nlu_engine_destroy_slots(CSlotList *result);

            [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            internal static extern SNIPS_RESULT snips_nlu_engine_destroy_string(string str);

            /// <summary>
            /// Used to retrieve the last error that happened in this thread. A function encountered an
            /// error if its return type is of type SNIPS_RESULT and it returned SNIPS_RESULT_KO.
            /// </summary>
            /// <returns></returns>
            [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            internal static extern SNIPS_RESULT snips_nlu_engine_get_last_error([In, Out] ref IntPtr error);

            [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            internal static extern SNIPS_RESULT snips_nlu_engine_get_model_version([In, Out] ref IntPtr version);

            //[DllImport("snips_nlu_ffi.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            //internal static extern unsafe SNIPS_RESULT snips_nlu_engine_run_get_intents(IntPtr client, string input, ref CIntentClassifierResultList result);

            [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            internal static extern SNIPS_RESULT snips_nlu_engine_run_get_intents_into_json(IntPtr client, string input, ref string result_json);

            //SNIPS_RESULT snips_nlu_engine_run_get_slots(const CSnipsNluEngine* client, const char* input, const char* intent, const CSlotList** result);

            [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            internal static extern SNIPS_RESULT snips_nlu_engine_run_get_slots_into_json(IntPtr client, string input, string intent, ref string result_json);

            //SNIPS_RESULT snips_nlu_engine_run_parse(const CSnipsNluEngine* client, const char* input, const CStringArray* intents_whitelist, const CStringArray* intents_blacklist, const CIntentParserResult** result);

            [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            internal static extern SNIPS_RESULT snips_nlu_engine_run_parse_into_json(IntPtr client, string input, CStringArray intents_whitelist, CStringArray intents_blacklist, ref string result_json);
        }
    }

    /// <summary>
    /// A function encountered an error if its return type is of type 
    /// SNIPS_RESULT and it returned SNIPS_RESULT_KO.
    /// </summary>
    public class NluEngineError : Exception
    {
        /// <summary>
        /// Used to retrieve the last error that happened in this thread. A function encountered an
        /// error if its return type is of type SNIPS_RESULT and it returned SNIPS_RESULT_KO.
        /// </summary>
        /// <returns></returns>
        internal static NluEngineError GetLast()
        {
            try
            {
                IntPtr errPrt = IntPtr.Zero;
                SNIPS_RESULT r = SnipsNLUEngine.NativeMethods.snips_nlu_engine_get_last_error(ref errPrt);
                if (r != SNIPS_RESULT.SNIPS_RESULT_OK)
                {
                    return new NluEngineError("Cannot retrieve last error.");
                }
                return new NluEngineError(Marshal.PtrToStringAnsi(errPrt));
            }
            catch (Exception ex)
            {
                return new NluEngineError("Cannot retrieve last error.", ex);
            }
        }

        internal NluEngineError(string message) : base(message)
        {

        }

        internal NluEngineError(string message, Exception innerException) : base(message, innerException)
        {

        }
    }

    /// <summary>
    /// Used as a return type of functions that can encounter errors.
    /// </summary>
    public enum SNIPS_RESULT
    {
        /// <summary>
        /// The function returned successfully.
        /// </summary>
        SNIPS_RESULT_OK = 0,

        /// <summary>
        /// The function encountered an error, you can retrieve it using the dedicated function.
        /// </summary>
        SNIPS_RESULT_KO = 1,
    }
}
