# 05 — Worker Service with KEDA Autoscaling

![CI](https://github.com/vignesh-joshi-dotnet/dotnet-project-vault/actions/workflows/05-keda-worker-ci.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Tests](https://img.shields.io/badge/tests-6%20passing-brightgreen)

## What this is

A queue-based worker that processes jobs and scales automatically in Kubernetes based on queue depth. Push 100 orders in, KEDA spins up 10 worker pods. Queue empties, pods shut down.

This is how you build cost-efficient background processing on Kubernetes.

## How it works

1. **OrderProducerSimulator** — continuously enqueues orders (1-5 every 2-5 seconds)
2. **InMemoryQueue** — simple concurrent queue with metrics (count, total enqueued, total processed)
3. **Worker** — polls queue every 500ms, processes each order (simulated work: 50-250ms)
4. **KEDA ScaledObject** — watches queue depth, creates/destroys pods as needed

## Try it locally

```bash
cd 05-keda-worker-net10
dotnet run
```

Watch the console as orders are produced and processed. In production, this would connect to Azure Service Bus or RabbitMQ instead of in-memory queue.

## Kubernetes deployment

The `k8s/` folder includes:
- **deployment.yaml** — Worker pod definition with resource requests/limits
- **scaledobject.yaml** — KEDA config (scale 0-10 pods, queue depth threshold: 5 messages)

Deploy to cluster:
```bash
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/scaledobject.yaml

# Watch it scale
kubectl get hpa -w
```

When the queue hits 5+ messages, KEDA triggers scale-up. When queue empties, pods scale down to 0 (saves money).

## Stack

| Component | Choice |
|-----------|--------|
| Worker type | .NET `BackgroundService` |
| Queue | In-memory for demo; plug in Service Bus/RabbitMQ for production |
| Metrics | Custom counters (enqueued, processed) |
| Scaling | KEDA ScaledObject with Service Bus trigger authentication |
| Tests | xUnit with concurrent queue tests |
| Container | Docker multi-stage |

## Tests

6 tests covering queue behavior:

```bash
dotnet test
```

- Enqueue, dequeue operations
- FIFO ordering
- Atomic counters (thread-safe)
- Empty queue handling

## Deployment

Docker:
```bash
docker build -t keda-worker .
docker run keda-worker
```

Railway:
1. Connect GitHub repo to railway.app
2. Set folder to `05-keda-worker-net10`
3. Railway deploys in 60 seconds (no Kubernetes needed for demo)

## Real-world use cases

- **Email dispatch** — process queued emails at scale
- **Image resizing** — convert uploads, scale by queue depth
- **Log aggregation** — parse and index logs from multiple sources
- **Batch reports** — generate reports on-demand with auto-scaling workers

## Notes

- The `ord` producer is intentionally async and bursty — simulates realistic load spikes
- KEDA requires Kubernetes 1.18+ with KEDA operator installed
- Thread-safe queue uses `ConcurrentQueue<T>` with `Interlocked` counters for metrics
- tests/KedaWorker.Tests