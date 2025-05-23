﻿using System.Threading.Tasks;
using Xunit;
using System;
using BlazorDexie.Test.Database;
using BlazorDexie.ObjUrl;
using System.Linq;
using BlazorDexie.Options;

namespace BlazorDexie.Test
{
    public class StoreDataTest : IAsyncLifetime
    {
        BlazorDexieOptions _blazorDexieOptions;
        private ObjectUrlService _objectUrlService;


        public StoreDataTest(BlazorDexieOptions blazorDexieOptions, ObjectUrlService objectUrlService)
        {
            _blazorDexieOptions = blazorDexieOptions;
            _objectUrlService = objectUrlService;
        }

        public ValueTask InitializeAsync()
        {
            return ValueTask.CompletedTask;
        }

        [Fact]
        public async Task AddBlob()
        {
            // arrange
            await using var db = CreateDb();

            // act
            var initialData = new byte[] { 213, 23, 55, 234, 54 };
            var key = Guid.NewGuid();
            await db.BlobData.AddBlob(initialData, key, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            var data = await db.BlobData.GetBlob(key, TestContext.Current.CancellationToken);

            Assert.NotNull(data);
            Assert.Equal(initialData, data);
        }

        [Fact]
        public async Task AddObjectUrl()
        {
            // arrange
            await using var db = CreateDb();

            // act
            var initialData = new byte[] { 213, 23, 55, 234, 54 };
            var initialObjectUrl = await _objectUrlService.Create(initialData, cancellationToken: TestContext.Current.CancellationToken);
            var key = Guid.NewGuid();
            await db.BlobData.AddObjectUrl(initialObjectUrl, key, TestContext.Current.CancellationToken);

            // assert
            var objectUrl = await db.BlobData.GetObjectUrl(key, TestContext.Current.CancellationToken);
            var data = await _objectUrlService.FetchData(objectUrl, TestContext.Current.CancellationToken);

            Assert.NotNull(data);
            Assert.Equal(initialData, data);
        }

        [Fact]
        public async Task PutBlob()
        {
            // arrange
            await using var db = CreateDb();

            // act
            var initalData = new byte[] { 213, 23, 55, 234, 54 };
            var key = Guid.NewGuid();
            await db.BlobData.PutBlob(initalData, key, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            var data = await db.BlobData.GetBlob(key, TestContext.Current.CancellationToken);

            Assert.NotNull(data);
            Assert.Equal(initalData, data);
        }

        [Fact]
        public async Task PutObjectUrl()
        {
            // arrange
            await using var db = CreateDb();

            // act
            var initialData = new byte[] { 213, 23, 55, 234, 54 };
            var initialObjectUrl = await _objectUrlService.Create(initialData, cancellationToken: TestContext.Current.CancellationToken);
            var key = Guid.NewGuid();
            await db.BlobData.PutObjectUrl(initialObjectUrl, key, TestContext.Current.CancellationToken);

            // assert
            var objectUrl = await db.BlobData.GetObjectUrl(key, TestContext.Current.CancellationToken);
            var data = await _objectUrlService.FetchData(objectUrl, TestContext.Current.CancellationToken);

            Assert.NotNull(data);
            Assert.Equal(initialData, data);
        }

        [Fact]
        public async Task GetBlob()
        {
            // arrange
            await using var db = CreateDb();

            var initalData = new byte[] { 213, 23, 55, 234, 54 };
            var key = Guid.NewGuid();
            await db.BlobData.PutBlob(initalData, key, cancellationToken: TestContext.Current.CancellationToken);

            // act
            var data = await db.BlobData.GetBlob(key, TestContext.Current.CancellationToken);

            // assert
            Assert.NotNull(data);
            Assert.Equal(initalData, data);
        }

        [Fact]
        public async Task GetObjectUrl()
        {
            // arrange
            await using var db = CreateDb();

            var initalData = new byte[] { 213, 28, 55, 234, 54 };
            var key = Guid.NewGuid();
            await db.BlobData.PutBlob(initalData, key, cancellationToken: TestContext.Current.CancellationToken);

            // act
            var objectUrl = await db.BlobData.GetObjectUrl(key, TestContext.Current.CancellationToken);

            // assert
            var data = await _objectUrlService.FetchData(objectUrl, TestContext.Current.CancellationToken);

            Assert.NotNull(data);
            Assert.Equal(initalData, data);
        }

        [Fact]
        public async Task GetPrimaryKeys()
        {
            // arrange
            await using var db = CreateDb();

            var initalData = new byte[] { 213, 28, 55, 234, 54 };
            var key = Guid.NewGuid();
            await db.BlobData.PutBlob(initalData, key, cancellationToken: TestContext.Current.CancellationToken);

            var initalData2 = new byte[] { 213, 28, 55, 234, 54 };
            var key2 = Guid.NewGuid();
            await db.BlobData.PutBlob(initalData2, key2, cancellationToken: TestContext.Current.CancellationToken);

            // act
            var primaryKeys = await db.BlobData.PrimaryKeys(TestContext.Current.CancellationToken);

            // assert
            Assert.Equal(2, primaryKeys.Length);
            Assert.Contains(key, primaryKeys);
            Assert.Contains(key2, primaryKeys);
        }

        [Fact]
        public async Task BulkAddBlob()
        {
            // arrange
            await using var db = CreateDb();

            // act
            var initialData = new byte[][] { [213, 23, 55, 234, 54], [23, 23, 44], [11, 22, 33] };
            var keys = Enumerable.Range(0, initialData.Length).Select(_ => Guid.NewGuid()).ToArray();

#pragma warning disable CS0618 // Type or member is obsolete
            await db.BlobData.BulkAddBlob(initialData, keys, cancellationToken: TestContext.Current.CancellationToken);
#pragma warning restore CS0618 // Type or member is obsolete

            // assert
            var data = await Task.WhenAll(keys.Select(async k => await db.BlobData.GetBlob(k)));

            Assert.NotNull(data);

            for (int i = 0; i < 2;  i++)
            {
                Assert.Equal(initialData[i], data[i]);
            }
        }

        private MyDb CreateDb()
        {
            var databaseId = Guid.NewGuid().ToString();
            return new MyDb(_blazorDexieOptions, databaseId);
        }

        public async ValueTask DisposeAsync()
        {
            await _objectUrlService.DisposeAsync().ConfigureAwait(false);
        }
    }
}
