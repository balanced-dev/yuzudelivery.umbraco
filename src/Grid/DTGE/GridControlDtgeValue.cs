using System;
using Newtonsoft.Json.Linq;
using Our.Umbraco.DocTypeGridEditor.Helpers;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Umbraco.GridData.Models;
using Skybrud.Umbraco.GridData.Models.Values;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Skybrud.Umbraco.GridData.Dtge
{
#if NETCOREAPP
    /// <summary>
    /// Class representing the value of a DocTypeGridEditor grid control. 
    /// </summary>
    public class GridControlDtgeValue : GridControlValueBase {

        #region Properties

        /// <summary>
        /// Gets a reference <see cref="IPublishedContent"/> of grid control.
        /// </summary>
        public IPublishedElement Content { get; }

        #endregion

        #region Constructors


        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="control"/>.
        /// </summary>
        /// <param name="control">An instance of <see cref="GridControl"/> representing the control.</param>
        protected GridControlDtgeValue(GridControl control, DocTypeGridEditorHelper helper) : base(control, control.JObject) {
            
            JObject value = control.JObject.GetObject("value");

            string docTypeAlias = value.GetString("dtgeContentTypeAlias");

            string contentValue = value.GetObject("value").ToString();

            string controlId = GetControlId(contentValue);

            Content = helper.ConvertValueToContent(controlId, docTypeAlias, contentValue);

        }

        public string GetControlId(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                return BitConverter.ToString(
                  md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input))
                ).Replace("-", String.Empty);
            }
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Gets a media value from the specified <paramref name="control"/>.
        /// </summary>
        /// <param name="control">The parent control.</param>
        public static GridControlDtgeValue Parse(GridControl control, DocTypeGridEditorHelper helper) {
            return control == null ? null : new GridControlDtgeValue(control, helper);
        }

        #endregion

    }
#else
/// <summary>
    /// Class representing the value of a DocTypeGridEditor grid control. 
    /// </summary>
    public class GridControlDtgeValue : GridControlValueBase {

    #region Properties

        /// <summary>
        /// Gets a reference <see cref="IPublishedContent"/> of grid control.
        /// </summary>
        public IPublishedElement Content { get; }

    #endregion

    #region Constructors


        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="control"/>.
        /// </summary>
        /// <param name="control">An instance of <see cref="GridControl"/> representing the control.</param>
        protected GridControlDtgeValue(GridControl control) : base(control, control.JObject) {
            
            JObject value = control.JObject.GetObject("value");

            string docTypeAlias = value.GetString("dtgeContentTypeAlias");

            string contentValue = value.GetObject("value").ToString();

            string controlId = GetControlId(contentValue);

            Content = DocTypeGridEditorHelper.ConvertValueToContent(controlId, docTypeAlias, contentValue);

        }

        public string GetControlId(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                return BitConverter.ToString(
                  md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input))
                ).Replace("-", String.Empty);
            }
        }

    #endregion

    #region Static methods

        /// <summary>
        /// Gets a media value from the specified <paramref name="control"/>.
        /// </summary>
        /// <param name="control">The parent control.</param>
        public static GridControlDtgeValue Parse(GridControl control) {
            return control == null ? null : new GridControlDtgeValue(control);
        }

    #endregion

    }
#endif
}