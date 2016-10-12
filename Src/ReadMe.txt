
无论在实施还是在产品版的开发中，客户端脚本的使用越来越频繁。随着一些经验的积累，最近我开发了一个工具，可以帮助用于简化脚本的声明，调用，使其与服务端代码、控件等进行隔离，以增强重用性，如果采用良好的编程实践，也可以避免页面全局域的名字污染。




下面举两个例子做一说明。




这个工具的核心类是 ScriptService。我们还是从著明的 Hello World 谈起。




首先，创作一个类从 ScriptService 继承（类文件是 GreetService.cs）：




    [Serializable]
     public sealed class GreetService : ScriptService

    {

        //...

    }








注意要加上 Serializable 标记，以使其有可能保存于页面视图状态中。然后为其生成服务 GUID（vs TOOLS -> Create GUID）：




    [Serializable]
     [ServiceGuid("A0BB6921-2D11-442D-B223-9A67F9A38653", Name = "Greet")]
     public sealed class GreetService : ScriptService


    {

        //...

    }





现在，我们再添加一个脚本文件 Greet.js：




(function ($) {
     function main(data) {

        //...


    }

    window['A0BB6921-2D11-442D-B223-9A67F9A38653'] = main;
 })(jQuery);




main 是入口函数，其在全局域中注册的 GUID 与服务类相同。我们要将这个脚本文件的生成属性设为嵌入资源（属性窗口，Build Action 选择 Embedded Resource）。再回到服务类，首先，我们将脚本文件声明为 Web Resource：




[assembly: WebResource("CIPACE.Extension.Greet.js", "text/javascript")]




namespace CIPACE.Extension
 {
     [Serializable]


    ...




然后，实现基类的抽象方法 Declare：




        protected override void Declare(Page host)
         {
             DeclareWebResource(host, "CIPACE.Extension.Greet.js", typeof(GreetService).Assembly);
         }





然后，再加两个需要传递到客户端的属性：




        [DataProperty("to", Required = true)]
         public string GreetTo
         { get; set; }




        [DataProperty("now")]
         public DateTime Now
         {
             get { return DateTime.Now; }
         }





标记 DataProperty 是可选的，如果不加，则客户端数据属性名与服务端相同。这样，服务端的代码就完成了。




客户端脚本的代码如下：




(function ($) {
     function main(data) {


        data.normalize(data);


         var h2$ = $('<h2>');
         h2$.text('Hello, ' + data.to + '! ' + '(' + data.now.toUTCString() + ')');

        var div$ = $('<div>');

        div$.css('text-align', 'center').append(h2$).appendTo('body');

    }

    window['A0BB6921-2D11-442D-B223-9A67F9A38653'] = main;
 })(jQuery);





至此，所的工作就完成了。在需要这个调用的页面（Default.aspx.cs）中，我们在 PreRender 阶段中实例化这个服务并调用即可：




        protected void Page_PreRender(object sender, EventArgs e)
         {
             GreetService greet = new GreetService();




            greet.GreetTo = "Lily";




            greet.Invoke(Page);
         }




当然，在我们的调用类中可以声明任何我们感兴趣的脚本服务（回调实质相同），以增强我们的调用能力。下面举一个简单的例子：




假如我们的客户端需要一个应用程序目录映射的服务，以使资源的路径保持正确。这个服务很简单，服务端代码是（MapAppPathService.cs）：




[assembly: WebResource("CIPACE.Extension.MapAppPath.js", "text/javascript")]




namespace CIPACE.Extension
{
    [Serializable]
    [ServiceGuid("C8835431-6285-4D38-9AD6-DB5EC27387E8", Name = "Map Web Application Path")]
    public sealed class MapAppPathService : ScriptService
    {
        private string _appPath;




        [DataProperty("appPath")]
        public string AppPath
        {
            get { return _appPath; }
        }

        protected override void Declare(Page host)
        {
            DeclareWebResource(host, "CIPACE.Extension.MapAppPath.js", typeof(MapAppPathService).Assembly);
        }




        protected override void Initialize(Page host)
        {
            _appPath = host.Request.ApplicationPath;
        }
    }
}




ScriptService 类提供了个 Initialize 虚方法，我们的类正好可以利用来获取应用程序路径。客户端代码如下（MapAppPath.js）：




(function ($) {
    function main(data) {
        var appPath = data.appPath;




        if (!appPath.endsWith('/')) {
            appPath += '/';
        }




        return function (path) {
            if ((typeof path !== 'string') || (path.length === 0)) {
                return path;
            }




            if (path.startsWith('/')) {
                path = path.substring(1);
            }




            return appPath + path;
        }
    }




    window['C8835431-6285-4D38-9AD6-DB5EC27387E8'] = main;
})(jQuery);




处理与前相同。注意，这个服务返回了一个函数。




我们在前面的 GreetService 类中加一个属性，就可以使用这个服务了：



         MapAppPathService _mapPath = new MapAppPathService();




        [DataProperty("mapPath")]
        public MapAppPathService MapPath
        {
            get
            {
                return _mapPath;
            }
        }





有脚本中，我们可以这样使用这个服务：

       

         //...

        // Append image
        var div2$ = $('<div>').css('text-align', 'center').appendTo('body');
        var img$ = $('<img>').attr('src', data.mapPath('Images/girl.jpg')).appendTo(div2$);

        //...




通过这样处理，可以实现服务优雅的调用，高可重用性与开发效率的提升。





补充说明：




1.如果服务类中一个属性不希望传递到客户端，则将其标记为 NonDataPropertyAttribute，如果属性是一个复杂类型，这个类型中某些属性不需要/不能（例如造成循环引用）传递到客户端 ，则可以将其标记为  System.Web.Script.Serialization.ScriptIgnoreAttribute
2.脚本声明也可以声明代码块（嵌入资源/串）和站点文件
