// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Specs.when_retrieving_committed_event_streams
{
    [Subject(typeof(IFetchCommittedEvents))]
    public class when_fetching_events_for_an_event_source_that_has_commits_from_a_given_commit_greater_than_committed : given.an_event_store
    {
        static IEventStore event_store;
        static CommittedEventStream first_commit;
        static CommittedEventStream second_commit;
        static CommittedEventStream third_commit;
        static UncommittedEventStream uncommitted_events;
        static EventSourceKey event_source;
        static DateTimeOffset? occurred;
        static Commits result;

        Establish context = () =>
        {
            event_store = get_event_store();
            event_source = get_event_source_key();
            occurred = DateTimeOffset.UtcNow.AddSeconds(-10);
            uncommitted_events = event_source.BuildUncommitted(occurred);
            event_store._do((es) => first_commit = es.Commit(uncommitted_events));
            uncommitted_events = first_commit.BuildNext(DateTimeOffset.UtcNow);
            event_store._do((es) => second_commit = es.Commit(uncommitted_events));
            uncommitted_events = second_commit.BuildNext(DateTimeOffset.UtcNow);
            event_store._do((es) => third_commit = es.Commit(uncommitted_events));
        };

        Because of = () => event_store._do((es) => result = es.FetchFrom(event_source, 4));

        It should_retrieve_empty_commits = () => result.Count().ShouldEqual(0);

        Cleanup nh = () => event_store.Dispose();
    }
}