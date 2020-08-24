namespace neko.host
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Neko.Base;
    using Neko.NativeRing;

    internal class Program
    {
        public static void tc()
        {
            Console.WriteLine("ITS WORK");
        }
        public static unsafe void Main(string[] args)
        {
            using var vm = new Neko();
            var module = vm.LoadModule(new FileInfo("test.n"));

            var xuy = (NekoArray)module["dws"].Invoke();


            module["assert_boolean"].InvokeWithNative(Native.v_true());
            module["assert_boolean"].InvokeWithNative(Native.v_false());

            var ftrue = module["assert_boolean_true"].Invoke();
            var ffalse = module["assert_boolean_false"].Invoke();

            var allc = NekoArray.Alloc(3);
            var allc2 = allc.AsInternal();

            var ffe1 = ftrue.@ref == Native.v_true();
            var ffe2 = ffalse.@ref == Native.v_false();

            module["test_array2"].InvokeWithNative(xuy.@ref);

            var xuy2 = xuy.AsInternal();

            var qw = xuy.GetByIndexNative(0);
            var tnull = Native.v_null();

            var x = (NekoArray)module["ttdarr"].Invoke();
            var d10 = x.AsInternal();
            var d11 = (_neko_array*)Native.neko_alloc_array(3);
            var d1 = module["new_empty_obj"].Invoke();
            var d2 = module["new_obj"].Invoke();
            if (d2 is NekoRuntimeObject obj)
            {
                var a = obj.AsDynamic();
                var strw = a.msg;

                var xu2y = obj.GetFields();
            }



            var f2 = module["assert_string"].Invoke("xuy");
            var f3 = module["assert_boolean"].Invoke<bool>(true);
            var f4 = module["assert_function"].Invoke<NekoFunction>(NekoFunction.Create(tc, nameof(tc))).Invoke();

            
        }


        public class NekoModuleResource
        {
            public static NekoModuleResource LoadByName(string name)
            {
                var assembly = typeof(NekoModuleResource).GetTypeInfo().Assembly;
                var resource = assembly.GetManifestResourceStream($"{assembly.GetName().Name}._modules.{name}.n");
                throw new NotImplementedException();
            }

            internal static bool IsCached() => 
                throw new NotImplementedException();

            private static string GetMD5() =>
                throw new NotImplementedException();
        }
    }
}
