using SPAD.neXt.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }

    /// <summary>
    /// CDU cell flags
    /// </summary>
    [Flags]
    public enum CDU_FLAG
    {
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
        UNUSED = 0x04,
    }

    public enum CDU_NUMBER
    {
        Left =  0,
        Captain = 0,
        Right = 1,
        FirstOfficer = 1,
        Center = 2,
        Custom = 99,
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
        Degree = 0xb0
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
        KEY_CLB,
        KEY_CRZ,
        KEY_DES,
        KEY_ATC,
        KEY_BRT_DIM
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
        public byte Symbol { get;  set; }

        /// <summary>
        /// Cell color <see cref="CDU_COLOR"/>
        /// </summary>
        public CDU_COLOR Color { get;  set; }

        /// <summary>
        /// Cell Flags <see cref="CDU_FLAG"/> 
        /// </summary>
        public CDU_FLAG Flags { get;  set; }

        public int Row { get; set; }
        public int Column { get;  set; }

        public bool IsEmpty
        {
            get { return (Symbol == '\0') || (Symbol == ' '); }
        }

        public CDU_Cell()
        {
        }

        public CDU_Cell(int row, int column,byte symbol, CDU_COLOR color, CDU_FLAG flags)
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

    public delegate void RenderCDUCellDelegate(int row, int col, byte symbol, CDU_COLOR color, CDU_FLAG flags);
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

        /// <summary>
        /// Get content of a CDU row
        /// </summary>
        /// <param name="rowNumber">Row number ( 0 - 13 )</param>
        /// <param name="startOffset">Starting offset ( 0 - 23 )</param>
        /// <param name="endOffset">End offset ( 0 - 23 )</param>
        /// <returns>Content of CDU row</returns>
        string GetColorRow(int rowNumber);
        string GetColorRow(int rowNumber, int startOffset);
        string GetColorRow(int rowNumber, int startOffset, int endOffset);
        string GetRow(int rowNumber);
        string GetRow(int rowNumber, int startOffset );
        string GetRow(int rowNumber, int startOffset , int endOffset );

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

        void GetCDUContent(RenderCDUCellDelegate renderCallback);
        void GetCDURow(int sourceRow, int targetRow, RenderCDUCellDelegate renderCallback);
        /// <summary>
        /// Send a key to the CDU
        /// </summary>
        /// <param name="key">Key to send <see cref="CDU_KEYS"/></param>
        void SendKey(CDU_KEYS key);

    }

    public abstract class GenericCDUScreen : ICDUScreen
    {
        protected List<List<byte>> EmptyScreen;
        protected Dictionary<CDU_LED, int> LedStatus = new Dictionary<CDU_LED, int>();
        protected ILogger logger;
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        public bool IsValid { get; set; }
        public string Provider { get; set; }
        //public ICDUValueProvider ValueProvider { get; set; }
        protected IApplication ApplicationProxy { get; private set; }

        protected CDU_Cell[] Cells;

        public abstract CDU_NUMBER CDUNumber { get; }
        public abstract bool IsPowered { get; }
        public abstract void SendKey(CDU_KEYS key);

        public abstract void ImportData();
        public GenericCDUScreen(int rows, int cols, IApplication appProxy)
        {
            ApplicationProxy = appProxy;
            logger = ApplicationProxy?.GetLogger("Aircraft.CDU");
            RowCount = rows;
            ColumnCount = cols;
            Cells = new CDU_Cell[rows * cols];
            EmptyScreen = new List<List<byte>>(rows * cols);
            for (int i = 0; i < rows * cols; i++)
            {
                EmptyScreen.Add(null);
            }
            foreach (CDU_LED item in Enum.GetValues(typeof(CDU_LED)))
            {
                LedStatus[item] = 0;
            }
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
            LedStatus[led] = isOn;
            ApplicationProxy?.CurrentAircraft?.UpdateLedStatus(CDUNumber, led, isOn);
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

        public void GetCDUContent(RenderCDUCellDelegate renderCallback)
        {
            for (int row = 0; row < RowCount; row++)
            {
                for (int col = 0; col < ColumnCount; col++)
                {
                    var colData = Cells[row * ColumnCount + col];
                    renderCallback(row, col, colData.Symbol, colData.Color, colData.Flags);
                }
            }
        }

        public void GetCDURow(int sourceRow, int targetRow,RenderCDUCellDelegate renderCallback)
        {
            for (int col = 0; col < ColumnCount; col++)
            {
                var colData = Cells[sourceRow * ColumnCount + col];
                renderCallback(targetRow, col, colData.Symbol, colData.Color, colData.Flags);
            }
        }

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
                return String.Empty;
            String retStr = "";
            for (int i = startOffset; i <= endOffset; i++)
            {
                retStr += (char)Cells[rowNumber * ColumnCount + i].Symbol;
            }
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
