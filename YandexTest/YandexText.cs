using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace YandexTest
{
    [TestClass]
    public class YandexText
    {
        private ChromeDriver _chrome;

        [TestMethod]
        public void Test_Yandex()
        {
            //alert

            string yandexURL = "http://www.yandex.ru/";
            int page = 2;
            string fileName = "D:\\output.txt";
            Regex regex = new Regex(@"yabs\.yandex|yandex\.ru/video|yandex\.ru/images");
            
            //открываю GoogleChrome на весь экран
            _chrome = new ChromeDriver();
            _chrome.Manage().Window.Maximize();

            //перехожу на главную страницу Яндекс
            _chrome.Navigate().GoToUrl(yandexURL);

            //act

            //устанавливаю фокус на строке поиска и ввожу запрос
            IWebElement text = _chrome.FindElementById("text");
            text.SendKeys("автомобили ваз" + Keys.Enter);

            for (int i = 0; i < page; ++i)
            {
                //ищу все элементы-резултаты поиска, содержащие конечные URL
                ReadOnlyCollection<IWebElement> foundResult =
                    _chrome.FindElements(By.CssSelector("div.content__left  ul li h2 a"));

                //отсев рекламы и ссылкок с видео и изображениями by yandex, т.е основные
                var str = foundResult
                    .Select(x => x.GetAttribute("href"))
                    .Where(h=>!regex.IsMatch(h))
                    .ToList();

                //дополнять существующий файл, или создать новый
                if (File.Exists(fileName))
                {
                    using (StreamWriter sw = File.AppendText(fileName))
                    {
                        sw.WriteLine("From {0} page:", i + 1);
                        foreach (var url in str)
                            sw.WriteLine(url);
                    }
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(fileName))
                    {
                        sw.WriteLine("From {0} page:", i + 1);
                        foreach (var url in str)
                            sw.WriteLine(url);
                    }
                }
                //переход на следующие страницы
                IWebElement numbPage = _chrome.FindElement(By.CssSelector("div.pager.i-bem.pager_js_inited  a"));
                _chrome.Navigate().GoToUrl(numbPage.GetAttribute("href"));
            }
        }

        [TestCleanup]
        public void TearDown()
        {
            _chrome.Quit();
        }
    }
}
