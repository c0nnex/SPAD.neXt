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
        public char Symbol { get;  set; }

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

        /// <summary>
        /// Subscribable event if content of CDU screen changed
        /// </summary>
        event SPADEventHandler<ICDUScreen,EventArgs> CDUChanged;

        /// <summary>
        /// Identifier of this CDU
        /// </summary>
        CDU_NUMBER CDUNumber { get; }

        /// <summary>
        /// Get content of a CDU row
        /// </summary>
        /// <param name="rowNumber">Row number ( 0 - 13 )</param>
        /// <param name="startOffset">Starting offset ( 0 - 23 )</param>
        /// <param name="endOffset">End offset ( 0 - 23 )</param>
        /// <returns>Content of CDU row</returns>
        string GetRow(int rowNumber, int startOffset = 0, int endOffset = 23);

        /// <summary>
        /// Get content of a CDU column
        /// </summary>
        /// <param name="colNumber">Column number ( 0 - 23 )</param>
        /// <returns>Content of CDU column of all rows</returns>
        string GetCol(int colNumber);

        /// <summary>
        /// Get a single CDU cell value
        /// </summary>
        /// <param name="rowNumber">Row number ( 0 - 13 )</param>
        /// <param name="colNumber">Column number ( 0 - 23 )</param>
        /// <returns><see cref="CDU_Cell"/> with cell content</returns>
        CDU_Cell GetCell(int rowNumber, int colNumber);

        /// <summary>
        /// Send a key to the CDU
        /// </summary>
        /// <param name="key">Key to send <see cref="CDU_KEYS"/></param>
        void SendKey(CDU_KEYS key);

    }
}
