﻿#pragma checksum "C:\Users\10565\Desktop\Photo\Photo\View\CameraButton\video.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4EB098C0197BA7BD3C0B1B9623870407"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Photo.View.CameraButton
{
    partial class video : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.PreviewControl = (global::Windows.UI.Xaml.Controls.CaptureElement)(target);
                }
                break;
            case 2:
                {
                    this.VideoButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 25 "..\..\..\..\View\CameraButton\video.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.VideoButton).Click += this.VideoButton_Click;
                    #line default
                }
                break;
            case 3:
                {
                    this.start = (global::Windows.UI.Xaml.Shapes.Ellipse)(target);
                }
                break;
            case 4:
                {
                    this.stop = (global::Windows.UI.Xaml.Shapes.Rectangle)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

