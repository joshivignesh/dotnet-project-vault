type JobPosting = {
  id: string;
  title: string;
  company: string;
  location: string;
  workMode: string;
  postedOn: string;
};

const apiBaseUrl =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5188";

async function getJobs(): Promise<JobPosting[]> {
  const response = await fetch(`${apiBaseUrl}/api/jobs`, {
    cache: "no-store",
  });

  if (!response.ok) {
    return [];
  }

  return (await response.json()) as JobPosting[];
}

export default async function Home() {
  const jobs = await getJobs();

  return (
    <main className="min-h-screen bg-slate-50 px-6 py-12 text-slate-900">
      <div className="mx-auto max-w-4xl">
        <p className="mb-3 text-sm font-semibold uppercase tracking-wide text-blue-700">
          FULLSTACK-01
        </p>
        <h1 className="text-4xl font-bold tracking-tight sm:text-5xl">
          JobBoard Starter
        </h1>
        <p className="mt-4 max-w-2xl text-base text-slate-600">
          Minimal Next.js frontend wired to the .NET 10 backend.
        </p>

        <section className="mt-10 grid gap-4">
          {jobs.length === 0 ? (
            <div className="rounded-xl border border-amber-300 bg-amber-50 p-4 text-amber-900">
              No jobs returned from API at {apiBaseUrl}. Start the backend and
              refresh.
            </div>
          ) : (
            jobs.map((job) => (
              <article
                key={job.id}
                className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm"
              >
                <h2 className="text-xl font-semibold">{job.title}</h2>
                <p className="mt-1 text-slate-700">{job.company}</p>
                <div className="mt-3 flex flex-wrap gap-2 text-sm text-slate-600">
                  <span className="rounded-full bg-slate-100 px-3 py-1">
                    {job.location}
                  </span>
                  <span className="rounded-full bg-slate-100 px-3 py-1">
                    {job.workMode}
                  </span>
                  <span className="rounded-full bg-slate-100 px-3 py-1">
                    Posted: {job.postedOn}
                  </span>
                </div>
              </article>
            ))
          )}
        </section>
      </div>
    </main>
  );
}
