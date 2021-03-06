﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Artifacts;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Specs.when_fetching_events_for_an_event_type
{
    [Subject(typeof(IFetchCommittedEvents))]
    [Tags("michael")]
    public class that_has_committed_events : given.an_event_store
    {
        static IEventStore event_store;
        static CommittedEventStream first_commit;
        static CommittedEventStream second_commit;
        static UncommittedEventStream uncommitted_events;
        static EventSourceKey event_source;
        static DateTimeOffset? occurred;
        static SingleEventTypeEventStream result;

        static List<EventEnvelope> simple_events;

        static ArtifactId simple_event_artifact;

        Establish context = () =>
        {
            simple_events = new List<EventEnvelope>();
            event_store = get_event_store();
            simple_event_artifact = event_artifacts[typeof(SimpleEvent)];
            event_source = get_event_source_key();
            occurred = DateTimeOffset.UtcNow.AddSeconds(-10);
            uncommitted_events = event_source.BuildUncommitted(occurred);
            simple_events.AddRange(uncommitted_events.Events.FilteredByEventType(simple_event_artifact));
            event_store._do((es) => first_commit = es.Commit(uncommitted_events));
            uncommitted_events = first_commit.BuildNext(DateTimeOffset.UtcNow);
            simple_events.AddRange(uncommitted_events.Events.FilteredByEventType(simple_event_artifact));
            event_store._do((es) => second_commit = es.Commit(uncommitted_events));
        };

        Because of = () => result = event_store.FetchAllEventsOfType(simple_event_artifact);

        It should_retrieve_all_the_events_for_the_event_event_type = () => result.Count().ShouldEqual(4);
        It should_retrieve_the_events_in_commit_order = () => result.ShouldBeInOrder();

        It should_contain_all_the_events_of_the_specified_type = () => result.Select(e => e.ToEventEnvelope()).ShouldEqual(simple_events);

        Cleanup nh = () => event_store.Dispose();
    }
}