using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompanyName.Shared.Common.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.Shared.Tests
{
    [TestClass]
    public class TestPaging
    {
        [TestMethod]
        public void TestPageCount()
        {
            var mpl = new ModelPagingLinks("/root", new PagingParameters { Page = 1, PageSize = 5 }, 50, "&info=more");
            Assert.AreEqual(mpl.Pages[1], "/root?info=more&pageSize=5&page=1");
            Assert.AreEqual(mpl.Pages.Keys.Count, 10);
        }
        [TestMethod]
        public void TestNullQuery()
        {
            var mpl = new ModelPagingLinks("/root", new PagingParameters { Page = 1, PageSize = 5 }, 50, null);
            Assert.AreEqual(mpl.Pages[1], "/root?pageSize=5&page=1");
            Assert.AreEqual(mpl.Pages.Keys.Count, 10);
        }
        [TestMethod]
        public void TestEmptyQuery()
        {
            var mpl = new ModelPagingLinks("/root", new PagingParameters { Page = 1, PageSize = 5 }, 50, string.Empty);
            Assert.AreEqual(mpl.Pages[1], "/root?pageSize=5&page=1");
        }
        [TestMethod]
        public void TestQuery()
        {
            var mpl = new ModelPagingLinks("/root", new PagingParameters { Page = 1, PageSize = 5 }, 50, "test=1&test=2&page=4");
            Assert.AreEqual(mpl.Pages[1], "/root?test=1&test=2&pageSize=5&page=1");
        }
        [TestMethod]
        public void TestQuery2Pages()
        {
            var mpl = new ModelPagingLinks("/root", new PagingParameters { Page = 1, PageSize = 10 }, 15, "?pageSize=10&page=1&test=x");
            Assert.AreEqual(mpl.Pages[1], "/root?test=x&pageSize=10&page=1");
        }
    }
}
