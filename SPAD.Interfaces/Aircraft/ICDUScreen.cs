using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Aircraft.CDU
{
    /// <summary>
    /// CDU cell color
    /// </summary>
    public enum CDU_COLOR
    {
        WHITE = 0,
        CYAN = 1,
        GREEN = 2,
        MAGENTA = 3,
        AMBER = 4,
        RED = 5,
        BLUE = 6,
        DARKGREEN = 7,
    }

    /// <summary>
    /// CDU cell flags
    /// </summary>
    [Flags]
    public enum CDU_FLAG
    {
        NONE = 0x0,
        /// <summary>
        /// small font, including that used for line headers
        /// </summary>
        SMALL_FONT = 0x01,
        /// <summary>
        /// character background is highlighted in reverse video
        /// </summary>
        REVERSE = 0x02,
        /// <summary>
        /// dimmed character color indicating inop/unused entries
        /// </summary>
        INOP = 0x04,
        BIG_FONT = 0x06,
    }

    public enum CDU_NUMBER
    {
        Left = 0,
        Captain = 0,
        Right = 1,
        FirstOfficer = 1,
        Center = 2,
        Custom = 99,
    }

    public enum CDU_ROW_JUSTIFY
    {
        Left,
        Right,
        Center
    }

    public enum CDU_LED
    {
        CALL,
        FAIL,
        MSG,
        OFST,
        EXEC,
    }

    public enum CDU_SPECIAL_CHAR : byte
    {
        BallotBox = 0xea,
        ArrowUp = 0xa3,
        ArrowDown = 0xa4,
        ArrowLeft = 0xa1,
        ArrowRight = 0xa2,
        Degree = 0xb0,
        Celsius = 0xb1,
        Fahrenheit = 0xb2,
        Checkmark = 0xb3,
        Cursor = 0xb4
    }
    public enum CDU_MODE
    {
        MODE_BOEING,
        MODE_AIRBUS,
        MODE_CRJ
    }

    public enum CDU_KEYS
    {
        KEY_L1,
        KEY_L2,
        KEY_L3,
        KEY_L4,
        KEY_L5,
        KEY_L6,
        KEY_R1,
        KEY_R2,
        KEY_R3,
        KEY_R4,
        KEY_R5,
        KEY_R6,
        KEY_INIT_REF,
        KEY_RTE,
        KEY_DEP_ARR,
        KEY_ALTN,
        KEY_VNAV,
        KEY_N1_LIMIT,
        KEY_FIX,
        KEY_LEGS,
        KEY_HOLD,
        KEY_FMCCOMM,
        KEY_PROG,
        KEY_EXEC,
        KEY_MENU,
        KEY_NAV_RAD,
        KEY_PREV_PAGE,
        KEY_NEXT_PAGE,
        KEY_0,
        KEY_1,
        KEY_2,
        KEY_3,
        KEY_4,
        KEY_5,
        KEY_6,
        KEY_7,
        KEY_8,
        KEY_9,
        KEY_DOT,
        KEY_PLUS_MINUS,
        KEY_A,
        KEY_B,
        KEY_C,
        KEY_D,
        KEY_E,
        KEY_F,
        KEY_G,
        KEY_H,
        KEY_I,
        KEY_J,
        KEY_K,
        KEY_L,
        KEY_M,
        KEY_N,
        KEY_O,
        KEY_P,
        KEY_Q,
        KEY_R,
        KEY_S,
        KEY_T,
        KEY_U,
        KEY_V,
        KEY_W,
        KEY_X,
        KEY_Y,
        KEY_Z,
        KEY_SPACE,
        KEY_DEL,
        KEY_SLASH,
        KEY_CLR,
        /* AErosoft CRJ */
        KEY_CLB,
        KEY_CRZ,
        KEY_DES,
        KEY_BRT_DIM,
        /* Start Airbus */
        KEY_DIR,
        KEY_PERF,
        KEY_INIT,
        KEY_DATA,
        KEY_FPLN,
        KEY_RAD,
        KEY_FUEL,
        KEY_SEC,
        KEY_ATC,
        KEY_AIRPORT,
        KEY_UP,
        KEY_DOWN,
        KEY_DIV,
        KEY_OVFLY,
        KEY_BRT,
        KEY_DIM
    }

    /// <summary>
    /// A single CDU cell
    /// </summary>
    public sealed class CDU_Cell
    {
        /// <summary>
        /// Cell Character
        /// (hex A1 / dec 161) is a left arrow 
        /// (hex A2 / dec 162) is a right arrow
        /// </summary>
        public byte Symbol { get; set; }

        /// <summary>
        /// Cell color <see cref="CDU_COLOR"/>
        /// </summary>
        public CDU_COLOR Color { get; set; }

        /// <summary>
        /// Cell Flags <see cref="CDU_FLAG"/> 
        /// </summary>
        public CDU_FLAG Flags { get; set; }

        public int Row { get; set; }
        public int Column { get; set; }

        public bool IsEmpty
        {
            get { return (Symbol == '\0') || (Symbol == ' '); }
        }

        public CDU_Cell()
        {
        }

        public CDU_Cell(int row, int column, byte symbol, CDU_COLOR color, CDU_FLAG flags)
        {
            Symbol = symbol;
            Color = color;
            Flags = flags;
            Row = row;
            Column = column;
        }

        public List<byte> ToArray() => Symbol == '\0' || Symbol == ' ' ? new List<byte>() : new List<byte> { (byte)Symbol, (byte)Color, (byte)Flags };
    }

    public sealed class CduLedData
    {
        public CDU_NUMBER CduNumber;
        public CDU_LED Led;
        public ICDUScreen Screen;
    }


    /// <summary>
    /// Interface to read CDU content (if supported by aircraft)
    /// </summary>
    public interface ICDUScreen
    {

        /// <summary>
        /// Powerstatus of CDU
        /// </summary>
        bool IsPowered { get; }

        /// <summary>
        /// CDU Data is valid (provided)
        /// </summary>
        bool IsValid { get; }
        int RowCount { get; }
        int ColumnCount { get; }
        List<List<byte>> ToArray();

        /// <summary>
        /// Identifier of this CDU
        /// </summary>
        CDU_NUMBER CDUNumber { get; }

        int GetLedStatus(CDU_LED led);
        void SetLedStatus(CDU_LED led, int isOn);

        void ShowMessage(string msg, int duration = 30);

        /// <summary>
        /// Get content of a CDU row
        /// </summary>
        /// <param name="rowNumber">Row number ( 0 - 13 )</param>
        /// <returns>Content of CDU row</returns>
        string GetColorRow(int rowNumber);
        string GetColorRow(int rowNumber, int startOffset);
        string GetColorRow(int rowNumber, int startOffset, int endOffset);
        string GetRow(int rowNumber);
        string GetRow(int rowNumber, int startOffset);
        string GetRow(int rowNumber, int startOffset, int endOffset);
        string GetParserRow(int rowNumber);

        /*        /// <summary>
                /// Get content of a CDU column
                /// </summary>
                /// <param name="colNumber">Column number ( 0 - 23 )</param>
                /// <returns>Content of CDU column of all rows</returns>
                string GetCol(int colNumber);
        */
        /// <summary>
        /// Get a single CDU cell value
        /// </summary>
        /// <param name="rowNumber">Row number ( 0 - 13 )</param>
        /// <param name="colNumber">Column number ( 0 - 23 )</param>
        /// <returns><see cref="CDU_Cell"/> with cell content</returns>
        CDU_Cell GetCell(int rowNumber, int colNumber);

        void GetCDUContent(ICDURenderer renderCallback);
        void GetCDURow(int sourceRow, int targetRow, ICDURenderer renderCallback);
        /// <summary>
        /// Send a key to the CDU
        /// </summary>
        /// <param name="key">Key to send <see cref="CDU_KEYS"/></param>
        void SendKey(CDU_KEYS key);
        bool RenderScratchPad(ICDURenderer renderCallback);
    }

    public interface ICDUDisplayRenderer
    {
        void RenderChar(int row, int col, byte symbol, CDU_COLOR color, CDU_FLAG flags = CDU_FLAG.NONE);
        void Clear();
        void ClearRow(int row);
    }

    public interface ICDURenderer : ICDUDisplayRenderer
    {
        // Row on Device to Display ScratchPad
        int ScratchPadRow { get; } 
        int RowForScratchPad { get; }
        int RowCount { get; }
        int ColumnCount { get; }

        void RenderText(int row, string text, CDU_COLOR color, CDU_FLAG flags = CDU_FLAG.NONE, CDU_ROW_JUSTIFY justify = CDU_ROW_JUSTIFY.Center);
    }
    public abstract class GenericCDUScreen : ICDUScreen
    {
        protected List<List<byte>> EmptyScreen;
        private string EmptyRow = "";
        protected Dictionary<CDU_LED, int> LedStatus = new Dictionary<CDU_LED, int>();
        protected ILogger logger;
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        public bool IsValid { get; set; }
        public string Provider { get; set; }
        //public ICDUValueProvider ValueProvider { get; set; }
        protected IApplication ApplicationProxy { get; private set; }

        protected CDU_Cell[] Cells;
        public CDU_NUMBER CDUNumber { get; private set; } = CDU_NUMBER.Left;
        public virtual bool IsPowered { get; protected set; } = false;
        public abstract void SendKey(CDU_KEYS key);

        private string Message { get; set; } = String.Empty;
        DateTime MessageEndDisplayAt { get; set; } = DateTime.MinValue;
        CancellationTokenSource MessageTokenSource = null;
        private Task messageTask = null;
        CDU_FLAG MessageFLags = CDU_FLAG.NONE;

        public abstract void ImportData();
        public GenericCDUScreen(CDU_NUMBER cduNum, int rows, int cols, IApplication appProxy)
        {
            ApplicationProxy = appProxy;
            logger = ApplicationProxy?.GetLogger("Aircraft.CDU." + (int)cduNum);
            RowCount = rows;
            ColumnCount = cols;
            Cells = new CDU_Cell[rows * cols];
            for (int i = 0; i < rows * cols; i++)
            {
                Cells[i] = new CDU_Cell();
            }
            EmptyScreen = new List<List<byte>>(rows * cols);
            for (int i = 0; i < rows * cols; i++)
            {
                EmptyScreen.Add(null);
            }
            EmptyRow = "".PadRight(cols);
            foreach (CDU_LED item in Enum.GetValues(typeof(CDU_LED)))
            {
                LedStatus[item] = 0;
            }
            CDUNumber = cduNum;
        }

        public override string ToString()
        {
            return $"CDU {CDUNumber} IsValid {IsValid} IsPowered {IsPowered} Provider {Provider} Title {GetRow(0)}";
        }

        public int GetLedStatus(CDU_LED led)
        {
            return LedStatus[led];
        }

        public void SetLedStatus(CDU_LED led, int isOn)
        {
            if (isOn != LedStatus[led])
            {
                LedStatus[led] = isOn;
                ApplicationProxy?.CurrentAircraft?.UpdateLedStatus(CDUNumber, led, isOn);
            }
        }

        public List<List<byte>> ToArray()
        {
            if (!IsPowered)
                return EmptyScreen;
            var rval = new List<List<byte>>(RowCount * ColumnCount);
            for (int row = 0; row < RowCount; row++)
            {
                for (int col = 0; col < ColumnCount; col++)
                {
                    rval.Add(Cells[row * ColumnCount + col].ToArray());
                }
            }
            return rval;
        }

        public void GetCDUContent(ICDURenderer renderCallback)
        {
            for (int row = 0; row < Math.Min(RowCount, renderCallback.RowCount); row++)
            {
                for (int col = 0; col < Math.Min(ColumnCount, renderCallback.ColumnCount); col++)
                {
                    var colData = Cells[row * ColumnCount + col];
                    renderCallback.RenderChar(row, col, colData.Symbol, colData.Color, colData.Flags);
                }
            }
            RenderScratchPad(renderCallback);
        }

        public void ClearRow(int rowNumber)
        {
            for (int col = 0; col < ColumnCount; col++)
            {
                Cells[rowNumber * ColumnCount + col] = new CDU_Cell();
            }
        }

        public void GetCDURow(int sourceRow, int targetRow, ICDURenderer renderCallback)
        {
            for (int col = 0; col < ColumnCount; col++)
            {
                var colData = Cells[sourceRow * ColumnCount + col];
                renderCallback.RenderChar(targetRow, col, colData.Symbol, colData.Color, colData.Flags);
            }
        }

        public void ShowMessage(string msg, int duration = 30)
        {
            if (MessageTokenSource != null)
            {
                MessageTokenSource.Cancel();
                if (messageTask != null)
                    Task.WaitAll(new Task[] { messageTask }, 1000);
            }
            MessageTokenSource = new CancellationTokenSource();
            MessageTokenSource.CancelAfter(TimeSpan.FromSeconds(duration));
            Message = msg;
            MessageEndDisplayAt = DateTime.Now.AddSeconds(duration);
            MessageFLags = CDU_FLAG.NONE;

            var token = MessageTokenSource.Token;
            Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (MessageFLags.HasFlag(CDU_FLAG.REVERSE))
                        MessageFLags = CDU_FLAG.NONE;
                    else
                        MessageFLags = CDU_FLAG.REVERSE;
                    ApplicationProxy.CurrentAircraft.RenderCDU(CDUNumber);
                    await Task.Delay(500, token);
                }
                MessageEndDisplayAt = DateTime.MinValue;
                ApplicationProxy.CurrentAircraft.RenderCDU(CDUNumber);
            });
        }

        public virtual bool RenderScratchPad(ICDURenderer renderCallback)
        {
            if (MessageEndDisplayAt < DateTime.Now)
                return false;
            renderCallback.ClearRow(renderCallback.ScratchPadRow);
            renderCallback.RenderText(renderCallback.ScratchPadRow, Message, CDU_COLOR.AMBER, CDU_FLAG.REVERSE, CDU_ROW_JUSTIFY.Center);
            return true;
        }

        public void SetCell(int row, int col, CDU_Cell cellData) => Cells[row * ColumnCount + col] = cellData;

        public CDU_Cell GetCell(int rowNumber, int colNumber)
        {
            if (!IsValid || (rowNumber < 0) || (colNumber < 0) || (rowNumber >= RowCount) || (colNumber >= ColumnCount))
                return null;
            return Cells[rowNumber * ColumnCount + colNumber];
        }
        public string GetRow(int rowNumber) => GetRow(rowNumber, 0, ColumnCount - 1);
        public string GetRow(int rowNumber, int startOffset) => GetRow(rowNumber, startOffset, ColumnCount - 1);
        public string GetRow(int rowNumber, int startOffset, int endOffset)
        {
            if (!IsValid || (rowNumber < 0) || (startOffset < 0) || (startOffset >= ColumnCount) || (rowNumber >= RowCount) || (endOffset < 0) || (endOffset >= ColumnCount))
                return EmptyRow;
            String retStr = "";
            for (int i = startOffset; i <= endOffset; i++)
            {
                retStr += (char)Cells[rowNumber * ColumnCount + i].Symbol;
            }
            return retStr;
        }

        struct cduColorStack
        {
            public CDU_COLOR _color;
            public CDU_FLAG _flag;

            public bool IsEqual(CDU_COLOR col, CDU_FLAG flag) => col == _color && flag == _flag;
            public bool IsEqual(CDU_Cell cell) => cell.Color == _color && cell.Flags == _flag;

            public string StartTag => "{" + _color.ToString().ToLowerInvariant() + "}" + FlagStart;
            public string EndTag => "{end}" + FlagEnd;
            public string FlagStart
            {
                get
                {
                    var str = String.Empty;
                    if (_flag.HasFlag(CDU_FLAG.REVERSE))
                        str += "{rev}";
                    if (_flag.HasFlag(CDU_FLAG.SMALL_FONT))
                        str += "{small}";
                    if (_flag.HasFlag(CDU_FLAG.INOP))
                        str += "{inop}";
                    return str;
                }
            }
            public string FlagEnd
            {
                get
                {
                    var str = String.Empty;
                    if (_flag.HasFlag(CDU_FLAG.REVERSE))
                        str += "{end}";
                    if (_flag.HasFlag(CDU_FLAG.SMALL_FONT))
                        str += "{end}";
                    if (_flag.HasFlag(CDU_FLAG.INOP))
                        str += "{end}";
                    return str;
                }
            }
        }
        public string GetParserRow(int rowNumber)
        {
            if (!IsValid || (rowNumber < 0) || (rowNumber >= RowCount))
                return String.Empty;
            String retStr = "";

            var cell = Cells[rowNumber * ColumnCount]; // First Char
            cduColorStack curColor = new cduColorStack { _color = cell.Color, _flag = cell.Flags };
            if (rowNumber % 2 == 1)
                retStr = "{small}";
            retStr += curColor.StartTag;
            retStr += (char)cell.Symbol;

            for (int i = 1; i < ColumnCount; i++)
            {
                cell = Cells[rowNumber * ColumnCount + i];
                if (!curColor.IsEqual(cell))
                {
                    retStr += curColor.EndTag;
                    curColor = new cduColorStack { _color = cell.Color, _flag = cell.Flags };
                    retStr += curColor.StartTag;
                }
                retStr += (char)Cells[rowNumber * ColumnCount + i].Symbol;
            }
            retStr += curColor.EndTag;
            if (rowNumber % 2 == 1)
                retStr += "{end}"; // end small
            return retStr;
        }

        public string GetColorRow(int rowNumber) => GetColorRow(rowNumber, 0, ColumnCount - 1);
        public string GetColorRow(int rowNumber, int startOffset) => GetColorRow(rowNumber, startOffset, ColumnCount - 1);
        public string GetColorRow(int rowNumber, int startOffset, int endOffset)
        {
            if (!IsValid || (rowNumber < 0) || (startOffset < 0) || (startOffset >= ColumnCount) || (rowNumber >= RowCount) || (endOffset < 0) || (endOffset >= ColumnCount))
                return String.Empty;
            String retStr = "";
            for (int i = startOffset; i <= endOffset; i++)
            {
                retStr += (char)Cells[rowNumber * ColumnCount + i].Color;
            }
            return retStr;
        }
    }
}
