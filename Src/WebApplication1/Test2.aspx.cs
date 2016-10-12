using System;
using System.Web.UI;

using CIPACE.Extension;

namespace WebApplication1
{
    public partial class Test2 : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            //base.OnLoad(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            CalculationService cal = new CalculationService();

            cal.ContainerID = pnlCal.ClientID;
            cal.PostBack = CreatePostBack();

            cal.Invoke(Page);

            HelloService service = new HelloService();

            for (int i = 0; i < 3; i++)
            {
                service.DeclareAll(Page);
            }
        }

        private FunctionDeclaration CreatePostBack()
        {
            PostBackOptions options = new PostBackOptions(btnSubmit);
            string statements = Page.ClientScript.GetPostBackEventReference(options);

            FunctionDeclaration f = new FunctionDeclaration
                {
                    Statements = statements
                };

            return f;
        }

        protected void Unnamed_Click(object sender, EventArgs e)
        {
            HelloService service = new HelloService();

            service.Invoke(Page);
        }
    }
}