namespace vm_base_test
{
    using System;
    using System.IO;
    using System.Linq;
    using Neko.Base;
    using Neko.NativeRing;
    using NUnit.Framework;

    public class VMTests
    {
        private Neko vm;
        private NekoModule module;
        [OneTimeSetUp]
        public void Setup()
        {
            vm = new Neko();
            module = vm.LoadModule(new FileInfo("./unit.n"));
        }

        [OneTimeTearDown]
        public void Shutdown() 
            => (vm as IDisposable).Dispose();

        [Test]
        public void TestObjectType()
        {
            var obj = module["testObject"].Invoke();
            Assert.IsInstanceOf<NekoRuntimeObject>(obj);
        }

        [Test]
        public void TestLenFields()
        {
            var robj = module["testObject"].Invoke() as NekoRuntimeObject;
            Assert.NotNull(robj);
            Assert.AreEqual(4, robj.GetFields().Length);
        }
        [Test]
        public unsafe void TestFields()
        {
            var robj = module["testObject"].Invoke() as NekoRuntimeObject;
            Assert.NotNull(robj);
            Assert.AreEqual(
                new[]{"x","y", "text", "fn"}.OrderBy(x => x),
                robj.GetFields().OrderBy(x => x));
        }
        [Test]
        public unsafe void TestObjectValueInt()
        {
            var robj = module["testObject"].Invoke() as NekoRuntimeObject;
            Assert.NotNull(robj);
            Assert.AreEqual(-1, (int)robj.AsDynamic().y);
        }
        [Test]
        public void TestObjectValueString()
        {
            var robj = module["testObject"].Invoke() as NekoRuntimeObject;
            Assert.NotNull(robj);
            Assert.AreEqual("the text", (string)robj.AsDynamic().text);
        }
        [Test]
        public void CallAndGetInt()
        {
            var o = module["setget_int"].Invoke(1);
            Assert.NotNull(o);
            Assert.AreEqual(1, (int)(NekoInt32)o);
        }
        [Test]
        public void CallAndGetFloat()
        {
            var o = module["setget_float"].Invoke(14.4f);
            Assert.NotNull(o);
            Assert.IsNotInstanceOf<NekoNull>(o);
            Assert.AreEqual(14.4f, (float)o);
        }
        [Test]
        public void CallAndGetString()
        {
            var o = module["setget_string"].Invoke("foo");
            Assert.NotNull(o);
            Assert.AreEqual("foo", (string)(NekoString)o);
        }
        [Test]
        public void CallAndGetBool()
        {
            var o = module["setget_boolean"].Invoke(true);
            Assert.NotNull(o);
            Assert.AreEqual(true, (bool)(NekoBool)o);
            o = module["setget_boolean"].Invoke(false);
            Assert.AreEqual(false, (bool)(NekoBool)o);
        }
        [Test]
        public void CallRefFunction()
        {
            var f = NekoFunction.Create(Success, nameof(Success));
            module["getset_and_call_function"].Invoke(f);
        }

        public static void Success() => Assert.Pass();

        //[Test]
        public void CallAndGetArray()
        {
            var arr = NekoArray.Alloc(3);
            arr[0] = true;
            arr[1] = false;
            arr[2] = "test";

            var result = module["setget_array"].Invoke(arr);
            Assert.NotNull(result);
            Assert.IsNotInstanceOf<NekoNull>(result);
            Assert.IsInstanceOf<NekoArray>(result);
            var result_array = (NekoArray)result;

            Assert.AreEqual(3, result_array.Count);
            Assert.AreEqual(true, (bool)result_array.First());
        }
        [Test]
        public void GetArray()
        {
            var result = module["new_array"].Invoke();
            Assert.NotNull(result);
            Assert.IsNotInstanceOf<NekoNull>(result);
            Assert.IsInstanceOf<NekoArray>(result);
            var result_array = (NekoArray)result;
            Assert.AreEqual(4, result_array.Count);
            Assert.AreEqual("1", (string)result_array[0]);
            Assert.AreEqual(2.12f, (float)result_array[1]);
            Assert.AreEqual("test", (string)result_array[2]);
            Assert.AreEqual(true, (bool)result_array[3]);
        }
    }
}