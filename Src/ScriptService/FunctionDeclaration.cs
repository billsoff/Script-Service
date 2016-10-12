#region Write Log
/*==============================================================================
 * Function:     Build client function declaration.
 * Programmer:   Bill Song    billsong@digicentury.com    13660481521@139.com
 * Created Date: 8/29/2015
 * *********************************************************************
 * Modify Log:
 *  Modify Date    Modifier	           Modify Content
 * 
 *==============================================================================
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace CIPACE.Extension
{
    /// <summary>
    /// Builds client function declaration.
    /// </summary>
    [Serializable]
    public class FunctionDeclaration : ILiterallySerializable
    {
        #region Factory Methods

        /// <summary>
        /// Creates the control finder.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>Control finder function declaration.</returns>
        public static FunctionDeclaration CreateControlFinder(Control control)
        {
            return new FunctionDeclaration
                        {
                            Statements = String.Format(
                                                "return document.getElementById('{0}');",
                                                control.ClientID
                                            )
                        };
        }

        /// <summary>
        /// Creates the post back.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The post back function declaration.</returns>
        public static FunctionDeclaration CreatePostBack(PostBackOptions options)
        {
            return new FunctionDeclaration
                        {
                            Statements = String.Format(
                                                "{0};",
                                                options.TargetControl.Page.ClientScript.GetPostBackEventReference(options)
                                            )
                        };
        }

        #endregion

        #region Fields

        private IList<string> _parameterNames;
        private string _statements;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionDeclaration"/> class.
        /// </summary>
        public FunctionDeclaration()
        {
        }

        /// <summary>
        /// Provide parameter name list.
        /// </summary>
        public IList<string> ParameterNames
        {
            get { return _parameterNames; }
            set { _parameterNames = value; }
        }

        /// <summary>
        /// Provide statements.
        /// </summary>
        public string Statements
        {
            get { return _statements; }
            set { _statements = value; }
        }

        /// <summary>
        /// Build function declaration.
        /// </summary>
        /// <returns>Function declaration.</returns>
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append("function (");

            if ((_parameterNames != null) && (_parameterNames.Count != 0))
            {
                for (int i = 0; i < _parameterNames.Count; i++)
                {
                    if (i != 0)
                    {
                        buffer.Append(", ");
                    }

                    buffer.Append(_parameterNames[i]);
                }
            }

            buffer.Append(") { ");

            buffer.Append(_statements);

            buffer.Append(" }");

            return buffer.ToString();
        }
    }
}