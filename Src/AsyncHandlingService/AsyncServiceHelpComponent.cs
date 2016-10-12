#region Write Log
/*==============================================================================
 * Function:         
 * Programmer:  Bill Song    billsong@digicentury.com    13660481521@139.com
 * Created Date: 12//2014
 * *********************************************************************
 * Modify Log:
 *  Modify Date    Modifier	           Modify Content
 * 
 *==============================================================================
 */
#endregion

namespace CIPACE.Extension
{
    [AsyncService(ParameterType = null)]
    public sealed class AsyncServiceHelpComponent : AsyncServiceComponent
    {
        public override object Execute()
        {
            return "Help Content";
        }
    }
}