﻿/* Copyright 2010-2014 MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace MongoDB.Driver.Tests.Builders
{
    [TestFixture]
    public class IndexKeysBuilderTests
    {
        private MongoServer _server;
        private MongoDatabase _database;
        private MongoServerInstance _primary;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _server = LegacyTestConfiguration.Server;
            _database = LegacyTestConfiguration.Database;
            _primary = _server.Primary;
        }

        [Test]
        public void TestTextIndexCreation()
        {
            if (_primary.InstanceType != MongoServerInstanceType.ShardRouter)
            {
                if (_primary.Supports(FeatureId.TextSearchCommand))
                {
                    var collection = _database.GetCollection<BsonDocument>("test_text");
                    collection.Drop();
                    collection.CreateIndex(IndexKeys.Text("a", "b").Ascending("c"), IndexOptions.SetTextLanguageOverride("idioma").SetName("custom").SetTextDefaultLanguage("spanish"));
                    var indexes = collection.GetIndexes();
                    var index = indexes.RawDocuments.Single(i => i["name"].AsString == "custom");
                    Assert.AreEqual("idioma", index["language_override"].AsString);
                    Assert.AreEqual("spanish", index["default_language"].AsString);
                    Assert.AreEqual(1, index["key"]["c"].AsInt32);
                }
            }
        }
    }
}