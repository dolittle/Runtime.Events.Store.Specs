﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Specs.when_getting_the_next_version_for_an_event_source
{
    [Subject(typeof(IFetchEventSourceVersion))]
    public class for_an_event_source_with_a_single_commit : given.an_event_store
    {
        static IEventStore event_store;
        static UncommittedEventStream uncommitted_events;
        static EventSourceKey event_source;
        static DateTimeOffset? occurred;
        static EventSourceVersion version;

        Establish context = () =>
        {
            event_store = get_event_store();
            occurred = DateTimeOffset.UtcNow.AddSeconds(-10);
            event_source = get_event_source_key();
            uncommitted_events = event_source.BuildUncommitted(occurred);
            event_store._do(_ => _.Commit(uncommitted_events));
        };

        Because of = () => event_store._do((es) => version = es.GetNextVersionFor(event_source));

        It should_get_the_the_second_commit = () => version.Commit.ShouldEqual(2ul);
        It should_have_a_sequence_of_0 = () => version.Sequence.ShouldEqual(0u);
        It should_not_be_an_intial_version = () => version.ShouldNotEqual(EventSourceVersion.Initial);
        It should_not_be_no_version = () => version.ShouldNotEqual(EventSourceVersion.NoVersion);

        Cleanup nh = () => event_store.Dispose();
    }
}