﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.Documentation;
using DependencyInjection.FeatureTests.TestTypes;
using DependencyInjection.FeatureTests.XunitSupport;
using Xunit;

namespace DependencyInjection.FeatureTests {
    [DisplayOrder(5)]
    [DisplayName("Property dependencies")]
    public class PropertyTests {
        [DisplayName("Simple dependency")]
        [ForEachFramework]
        public void PropertyDependency(IFrameworkAdapter framework) {
            framework.Register<IService, IndependentService>();
            framework.Register<ServiceWithSimplePropertyDependency>();

            var component = framework.GetInstance<ServiceWithSimplePropertyDependency>();

            Assert.NotNull(component.Service);
            Assert.IsAssignableFrom<IndependentService>(component.Service);
        }

        [DisplayName("Optional by default")]
        [ForEachFramework]
        public void PropertyDependencyIsOptional(IFrameworkAdapter framework) {
            framework.Register<ServiceWithSimplePropertyDependency>();
            var component = framework.GetInstance<ServiceWithSimplePropertyDependency>();

            Assert.Null(component.Service);
        }
    }
}
