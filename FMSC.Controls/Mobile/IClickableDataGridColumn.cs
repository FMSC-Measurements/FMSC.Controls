using System.Drawing;

namespace FMSC.Controls
{
    interface IClickableDataGridColumn
    {
        void HandleMouseDown(int rowNum, System.Windows.Forms.MouseEventArgs mea);

        void HandleMouseUp(int rowNum, System.Windows.Forms.MouseEventArgs mea);

        void HandleMouseClick(int rowNum);
    }
}
