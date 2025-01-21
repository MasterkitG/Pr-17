using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Reflection;
using UnitTestEx;
using Assert = NUnit.Framework.Assert;

namespace UnitTestProject
{
    /// <summary>
    /// Summary description for FileStorageTest
    /// </summary>
    [TestClass]
    public class FileStorageTest
    {
        public const string MAX_SIZE_EXCEPTION = "DIFFERENT MAX SIZE";
        public const string NULL_FILE_EXCEPTION = "NULL FILE";
        public const string NO_EXPECTED_EXCEPTION_EXCEPTION = "There is no expected exception";

        public const string SPACE_STRING = " ";
        public const string FILE_PATH_STRING = "@D:\\JDK-intellij-downloader-info.txt";
        public const string CONTENT_STRING = "Some text";
        public const string REPEATED_STRING = "AA";
        public const string WRONG_SIZE_CONTENT_STRING = "TEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtext";
        public const string TIC_TOC_TOE_STRING = "tictoctoe.game";

        public const int NEW_SIZE = 5;

        public FileStorage storage = new FileStorage(NEW_SIZE);

        /* ПРОВАЙДЕРЫ */

        static object[] NewFilesData =
        {
            new object[] { new File(REPEATED_STRING, CONTENT_STRING) },
            new object[] { new File(SPACE_STRING, WRONG_SIZE_CONTENT_STRING) },
            new object[] { new File(FILE_PATH_STRING, CONTENT_STRING) }
        };

        static object[] FilesForDeleteData =
        {
            new object[] { new File(REPEATED_STRING, CONTENT_STRING), REPEATED_STRING },
            new object[] { null, TIC_TOC_TOE_STRING }
        };

        static object[] NewExceptionFileData = {
            new object[] { new File(REPEATED_STRING, CONTENT_STRING) }
        };

        /* Тестирование записи файла */
        [TestMethod, TestCaseSource(nameof(NewFilesData))]
        public void WriteTest() 
        {
            string fileName = "defaultName", content = "defaultContent";
            File file = new File(fileName, content);
            Assert.True(storage.Write(file));
            storage.DeleteAllFiles();
        }

        /* Тестирование записи дублирующегося файла */
        [TestMethod, TestCaseSource(nameof(NewExceptionFileData))]
        public void WriteExceptionTest() {
            string fileName = "defaultName", content = "defaultContent";
            File file = new File(fileName, content);
            bool isException = false;
            try
            {
                storage.Write(file);
                Assert.False(storage.Write(file));
                storage.DeleteAllFiles();
            } 
            catch (FileNameAlreadyExistsException)
            {
                isException = true;
            }
            Assert.True(isException, NO_EXPECTED_EXCEPTION_EXCEPTION);
        }

        /* Тестирование проверки существования файла */
        [TestMethod, TestCaseSource(nameof(NewFilesData))]
        public void IsExistsTest() {
            string fileName = "defaultName", content = "defaultContent";
            File file = new File(fileName, content);
            String name = file.GetFilename();
            Assert.False(storage.IsExists(name));
            try {
                storage.Write(file);
            } catch (FileNameAlreadyExistsException e) {
                Console.WriteLine(String.Format("Exception {0} in method {1}", e.GetBaseException(), MethodBase.GetCurrentMethod().Name));
            }
            Assert.True(storage.IsExists(name));
            storage.DeleteAllFiles();
        }

        /* Тестирование удаления файла */
        [TestMethod, TestCaseSource(nameof(FilesForDeleteData))]
        public void DeleteTest() {
            string fileName = "defaultName",content="defaultContent";
            File file = new File(fileName,content);
            storage.Write(file);
            Assert.True(storage.Delete(fileName));
        }

        /* Тестирование получения файлов */
        [TestMethod]
        public void GetFilesTest()
        {
            foreach (File el in storage.GetFiles()) 
            {
                Assert.NotNull(el);
            }
        }

        // Почти эталонный
        /* Тестирование получения файла */
        [TestMethod, TestCaseSource(nameof(NewFilesData))]
        public void GetFileTest() 
        {
            string fileName = "defaultName", content = "defaultContent";
            File expectedFile = new File(fileName, content);
            storage.Write(expectedFile);

            File actualfile = storage.GetFile(expectedFile.GetFilename());
            bool difference = actualfile.GetFilename().Equals(expectedFile.GetFilename()) && actualfile.GetSize().Equals(expectedFile.GetSize());

            Assert.IsFalse(!difference, string.Format("There is some differences in {0} or {1}", expectedFile.GetFilename(), expectedFile.GetSize()));
            storage.DeleteAllFiles();
        }

        //Новые добавленные тесты
        /*Тестирование удаления всех файлов */
        [TestMethod]
        public void DeleteAllFilesTest()
        {
            string fileName = "defaultName1",fileName2="defaultName2", content = "defaultContent1",content2="defaultContent2";
            File file = new File(fileName, content);
            File file2 = new File(fileName2, content2);

            storage.Write(file);
            storage.Write(file2);
            storage.DeleteAllFiles();

            Assert.IsFalse(storage.IsExists(fileName)&&storage.IsExists(fileName2));
        }

        /*Ттестирование на возможность спецсимволы в название*/
        [TestMethod]
        public void SpecialSimbolsInFileNameTest()
        {
            string fileName = "defaultName*", content = "defaultContent";
            File file = new File(fileName, content);
            try 
            {
               storage.Write(file);
               storage.DeleteAllFiles();
               Assert.Fail();
            }
            catch
            {
                Console.WriteLine(String.Format("File \"{0}\" has special characters in it is name", fileName));
            }
            
        }

        /*Тестирование превышения места хранилища*/
        [TestMethod]
        public void StorageSpaceExceededTest()
        {
            string fileName = "defaultName", content = "111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111";
            File file = new File(fileName, content);
            try
            {
                storage.Write(file);
                storage.DeleteAllFiles();
                Assert.Fail();
            }
            catch
            {
                Console.WriteLine(String.Format("There is not enough storage space"));
            };
        }
    }
}
