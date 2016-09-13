using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Gauges
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

    /// <summary>
    /// A single CDU cell
    /// </summary>
    public interface CDU_Cell
    {
        /// <summary>
        /// Cell Character
        /// (hex A1 / dec 161) is a left arrow 
        /// (hex A2 / dec 162) is a right arrow
        /// </summary>
        char Symbol { get; }

        /// <summary>
        /// Cell color <see cref="CDU_COLOR"/>
        /// </summary>
        CDU_COLOR Color { get; }

        /// <summary>
        /// Cell Flags <see cref="CDU_FLAG"/> 
        /// </summary>
        CDU_FLAG Flags { get; }
    }


    /// <summary>
    /// Interface to read CDU content (if supported by aircraft)
    /// </summary>
    public interface ICDUScreen
    {
        /// <summary>
        /// Powerstatus of CDU
        /// </summary>
        bool Powered { get; }

        CDU_NUMBER CDUNumber { get; }

        /// <summary>
        /// Get content of a CDU row
        /// </summary>
        /// <param name="rowNumber">Row number ( 0 - 13 )</param>
        /// <param name="startOffset">Starting offset ( 0 - 23 )</param>
        /// <param name="endOffset">End offset ( 0 - 23 ) , -1 = all remaining characters</param>
        /// <returns>Content of CDU row</returns>
        string GetRow(int rowNumber, int startOffset = 0, int endOffset = -1);

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
    }
}
