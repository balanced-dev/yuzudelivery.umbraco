﻿using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData.Converters;

#if NETCOREAPP
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Models;
using Skybrud.Umbraco.GridData.Models.Values;
using Our.Umbraco.DocTypeGridEditor.Helpers;
#else
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Values;
using Skybrud.Umbraco.GridData.Interfaces;
#endif

namespace Skybrud.Umbraco.GridData.Dtge {

    /// <summary>
    /// Grid converter for the DocTypeGridEditor.
    /// </summary>
    public class DtgeGridConverter : GridConverterBase 
    {
#if NETCOREAPP
        private readonly DocTypeGridEditorHelper helper;

        public DtgeGridConverter(DocTypeGridEditorHelper helper)
        {
            this.helper = helper;
        }

        /// <summary>
        /// Converts the specified <paramref name="token"/> into an instance of <see cref="IGridControlValue"/>.
        /// </summary>
        /// <param name="control">A reference to the parent <see cref="GridControl"/>.</param>
        /// <param name="token">The instance of <see cref="JToken"/> representing the control value.</param>
        /// <param name="value">The converted control value.</param>
        public override bool ConvertControlValue(GridControl control, JToken token, out IGridControlValue value) {
            value = null;
            if (IsDocTypeGridEditor(control.Editor)) {
                value = GridControlDtgeValue.Parse(control, helper);
            }
            return value != null;
        }
#else
        /// <summary>
        /// Converts the specified <paramref name="token"/> into an instance of <see cref="IGridControlValue"/>.
        /// </summary>
        /// <param name="control">A reference to the parent <see cref="GridControl"/>.</param>
        /// <param name="token">The instance of <see cref="JToken"/> representing the control value.</param>
        /// <param name="value">The converted control value.</param>
        public override bool ConvertControlValue(GridControl control, JToken token, out IGridControlValue value) {
            value = null;
            if (IsDocTypeGridEditor(control.Editor)) {
                value = GridControlDtgeValue.Parse(control);
            }
            return value != null;
        }
#endif

        /// <summary>
        /// Gets an instance <see cref="GridControlWrapper"/> for the specified <paramref name="control"/>.
        /// </summary>
        /// <param name="control">The control to be wrapped.</param>
        /// <param name="wrapper">The wrapper.</param>
        /*public override bool GetControlWrapper(GridControl control, out GridControlWrapper wrapper) {
            wrapper = null;
            if (IsDocTypeGridEditor(control.Editor)) {
                wrapper = control.GetControlWrapper<GridControlDtgeValue>();
            }
            return wrapper != null;

        }*/

        private bool IsDocTypeGridEditor(GridEditor editor) {

            // The editor may be NULL if it no longer exists in a package.manifest file
            if (editor == null) return false;

            const string view = "/App_Plugins/DocTypeGridEditor/Views/doctypegrideditor.html";

            return ContainsIgnoreCase(editor.View.Split('?')[0], view);

        }

    }

}