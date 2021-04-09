using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserGraphClient;

public class Searcher<TEntity, TKey> where TEntity: IEntity<TKey>
{
    private Func<TKey, Task<IEnumerable<TEntity>>> source;
    private Func<IEnumerable<TEntity>, IEnumerable<TEntity>> filter;
    private Func<TEntity, Task> sink;
    private Func<TEntity, TEntity, Task> link;

    public Searcher(Func<TKey, Task<IEnumerable<TEntity>>>  source,
        Func<IEnumerable<TEntity>, IEnumerable<TEntity>> filter,
        Func<TEntity, Task> sink,
        Func<TEntity, TEntity, Task> link)
    {
        this.source = source;
        this.filter = filter;
        this.sink = sink;
        this.link = link;
    }

    public async Task Start(TEntity seed, CancellationToken cancellationToken)
    {
        await this.sink(seed);
        var queue = new Queue<TEntity>();
        queue.Enqueue(seed);

        while (queue.Count() > 0)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var current = queue.Dequeue();
            var next = await this.source(current.Id);
            var candidates = this.filter(next);
            foreach(var candiate in candidates)
            {
                await this.sink(candiate);
                await this.link(current, candiate);
                queue.Enqueue(candiate);
            }
        }
    }




}