﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using FakeItEasy;
using System.Linq.Expressions;

namespace AutoMocker.Specs
{
    public class when_faking_recursively_and_no_default_constructor
    {
        static MockContainer<SubjectWithBadDependencies> graph;

        Establish context = () =>
        {
            graph = B.AutoMock<SubjectWithBadDependencies>();
            A.CallTo(() => graph.GetMock<IDependency>().whatever()).Returns("Faked!!");
            A.CallTo(() => graph.GetMock<ADependency>().GetString()).Returns("Faked As Well!!");
        };

        Because of = () => { };

        It should_return_fake_whatever = () =>
        {
            graph.Subject.GetSomethingElse().ShouldEqual("Faked!!");

        };

        It should_return_fake_getstring = () =>
        {
            graph.Subject.GetSomething().ShouldEqual("Faked As Well!!");
        };
    }

    public class when_faking_recursively
    {
        static MockContainer<ASubject> graph;

        Establish context = () =>
        {
            graph = B.AutoMock<ASubject>();
            A.CallTo(() => graph.GetMock<IDependency>().whatever()).Returns("Faked!!");
            A.CallTo(() => graph.GetMock<ADependency>().GetString()).Returns("Faked As Well!!");
        };

        Because of = () => { };

        It should_return_fake_whatever = () => {
            graph.Subject.GetSomethingElse().ShouldEqual("Faked!!");
            
        };

        It should_return_fake_getstring = () =>
        {
            graph.Subject.GetSomething().ShouldEqual("Faked As Well!!");
        };
    }

    public class when_faking_non_recursively
    {
        static MockContainer<ASubject> graph;

        Establish context = () =>
        {
            graph = B.AutoMock<ASubject>(null, false);
            A.CallTo(() => graph.GetMock<IDependency>().whatever()).Returns("Faked!!");
            A.CallTo(() => graph.GetMock<ADependency>().GetString()).Returns("Faked As Well!!");
        };

        Because of = () => { };

        It should_return_fake_whatever = () =>
        {
            graph.Subject.GetSomethingElse().ShouldEqual("Faked!!");

        };

        It should_return_fake_getstring = () =>
        {
            graph.Subject.GetSomething().ShouldEqual("Faked As Well!!");
        };
    }

    public class when_faking_property_dependencies_by_name
    {
        static MockContainer<PropertySubject> graph;

        Establish context = () =>
        {
            graph = B.AutoMock<PropertySubject>(p => p.NameMatches(name => name.EndsWith("Dependency")));
        };

        Because of = () => { };

        It should_inject_matching_properties = () => graph.Subject.ADependency.ShouldNotBeNull();

        It should_not_inject_non_matching_properties = () => graph.Subject.Something.ShouldBeNull();
    }

    public class when_faking_property_dependencies_by_type
    {
        static MockContainer<PropertySubject> graph;

        Establish context = () =>
        {
            graph = B.AutoMock<PropertySubject>(p => p.TypeMatches(type => type == typeof(IDependency)));
        };

        Because of = () => { };

        It should_inject_matching_properties = () => graph.Subject.Something.ShouldNotBeNull();

        It should_not_inject_non_matching_properties = () => graph.Subject.ADependency.ShouldBeNull();
    }

    public class NoConstructorDependency
    {
        private NoConstructorDependency()
        {
        }
    }

    public class BDependency
    {
        public virtual int GetInt()
        {
            return 0;
        }
    }

    public class ADependency
    {
        BDependency dep;
        IDependency idep;

        public ADependency(BDependency dep, IDependency nested)
        {
            this.dep = dep;
            this.idep = nested;
        }

        public virtual string GetString()
        {
            return "Stuff";
        }
    }

    public interface IDependency
    {
        string whatever();
    }

    public class ASubject
    {
        public ASubject(IDependency dep2, ADependency dep)
        {
            this.dep = dep;
            this.dep2 = dep2;
        }

        ADependency dep;
        IDependency dep2;

        public string GetSomething()
        {
            return dep.GetString();
        }

        public string GetSomethingElse()
        {
            return dep2.whatever();
        }
    }

    public class PropertySubject
    {
        public PropertySubject()
        {
        }

        public ADependency ADependency { get; set; }
        public IDependency Something { get; set; }
    }

    public class SubjectWithBadDependencies : ASubject
    {
        public SubjectWithBadDependencies(NoConstructorDependency dep, ADependency dep2, IDependency dep3)
            : base(dep3, dep2)
        {
        }
    }
}
