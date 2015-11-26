// The MIT License (MIT)
// 
// Copyright (c) 2015 Capsor Consulting AB
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace WindowsPhoneSample.Core.Tests
{
    [TestClass]
    public class IoCTests
    {
        [TestMethod]
        public void Register()
        {
            IoC ioc = new IoC();
            ioc.Register<IBartender, Bartender>();
            var actual = ioc.Resolve<IBartender>();
            Assert.IsInstanceOfType(actual, typeof(Bartender));
        }

        [TestMethod]
        public void RegisterInstance()
        {
            IoC ioc = new IoC();
            Bartender b = new Bartender();
            ioc.RegisterInstance<IBartender>(b);
            var actual = ioc.Resolve<IBartender>();
            Assert.AreSame(b, actual);
        }

        [TestMethod]
        public void TwoInterfacesOneImplementation()
        {
            IoC ioc = new IoC();
            ioc.Register<IBartender, Bartender>();
            ISommeliere bartenderAsSommeliere = (ISommeliere)ioc.Resolve<IBartender>();
            ioc.RegisterInstance(bartenderAsSommeliere);
            var bartender = ioc.Resolve<IBartender>();
            var sommeliere = ioc.Resolve<ISommeliere>();
            Assert.AreSame(bartender, sommeliere);
        }

        [TestMethod]
        public void UnregisterInstance()
        {
            IoC ioc = new IoC();
            ioc.Register<IBartender, Bartender>();
            IBartender bartender = ioc.Resolve<IBartender>();
            ioc.UnregisterInstance(bartender);
            bool exceptionWasThrown = false;
            try
            {
                ioc.Resolve<IBartender>();
            }
            catch (Exception )
            {
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
        }

        [TestMethod]
        public void RegisterTwiceOverwrites()
        {
            IoC ioc = new IoC();
            ioc.Register<IBartender, John>();
            IBartender john = ioc.Resolve<IBartender>();
            Assert.IsTrue(john is John);
            ioc.Register<IBartender, Jane>();
            IBartender jane = ioc.Resolve<IBartender>();
            Assert.IsTrue(jane is Jane);
            Assert.AreNotSame(john, jane);
        }

        public interface IBartender
        {
        }

        public interface ISommeliere
        {
        }

        public sealed class Bartender : IBartender, ISommeliere
        {
        }

        public class John : IBartender
        {
             
        }

        public class Jane : IBartender
        {

        }

    }
}
