"use client";

import { FormEvent, useEffect, useMemo, useState } from "react";

type JobPosting = {
  id: string;
  title: string;
  company: string;
  location: string;
  workMode: string;
  postedOn: string;
  createdBy: string;
};

type TokenResponse = {
  accessToken: string;
  tokenType: string;
  expiresInMinutes: number;
};

const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5188";
const tokenStorageKey = "jobboard.auth.token";

export default function Home() {
  const [token, setToken] = useState("");
  const [username, setUsername] = useState("demo");
  const [password, setPassword] = useState("demo");
  const [jobs, setJobs] = useState<JobPosting[]>([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [newJob, setNewJob] = useState({
    title: "",
    company: "",
    location: "",
    workMode: "Hybrid",
  });

  const isAuthenticated = useMemo(() => token.length > 0, [token]);

  useEffect(() => {
    const savedToken = window.localStorage.getItem(tokenStorageKey) ?? "";
    if (savedToken.length > 0) {
      setToken(savedToken);
    }
  }, []);

  useEffect(() => {
    if (!token) {
      setJobs([]);
      return;
    }

    void fetchJobs(token);
  }, [token]);

  async function fetchJobs(currentToken: string) {
    setLoading(true);
    setError("");

    const response = await fetch(`${apiBaseUrl}/api/jobs`, {
      headers: {
        Authorization: `Bearer ${currentToken}`,
      },
      cache: "no-store",
    });

    if (!response.ok) {
      setError("Could not load jobs. Please log in again.");
      setLoading(false);
      return;
    }

    const data = (await response.json()) as JobPosting[];
    setJobs(data);
    setLoading(false);
  }

  async function onLogin(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");

    const response = await fetch(`${apiBaseUrl}/api/auth/token`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ username, password }),
    });

    if (!response.ok) {
      setError("Login failed. Use demo / demo for local testing.");
      return;
    }

    const payload = (await response.json()) as TokenResponse;
    window.localStorage.setItem(tokenStorageKey, payload.accessToken);
    setToken(payload.accessToken);
  }

  async function onCreateJob(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const response = await fetch(`${apiBaseUrl}/api/jobs`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(newJob),
    });

    if (!response.ok) {
      setError("Failed to create job posting.");
      return;
    }

    setNewJob({ title: "", company: "", location: "", workMode: "Hybrid" });
    await fetchJobs(token);
  }

  function onLogout() {
    window.localStorage.removeItem(tokenStorageKey);
    setToken("");
    setJobs([]);
  }

  return (
    <main className="min-h-screen bg-slate-50 px-6 py-12 text-slate-900">
      <div className="mx-auto max-w-5xl">
        <p className="mb-3 text-sm font-semibold uppercase tracking-wide text-blue-700">FULLSTACK-01</p>
        <h1 className="text-4xl font-bold tracking-tight sm:text-5xl">JobBoard Vertical Slice</h1>
        <p className="mt-4 max-w-2xl text-base text-slate-600">
          Token-secured recruiter workflow backed by .NET 10 + EF Core SQL persistence.
        </p>

        {!isAuthenticated ? (
          <section className="mt-10 rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
            <h2 className="text-xl font-semibold">Recruiter Login</h2>
            <p className="mt-2 text-sm text-slate-600">Local demo credentials: demo / demo</p>
            <form className="mt-4 grid gap-4 sm:grid-cols-2" onSubmit={onLogin}>
              <input
                className="rounded-lg border border-slate-300 px-3 py-2"
                value={username}
                onChange={(event) => setUsername(event.target.value)}
                placeholder="Username"
              />
              <input
                className="rounded-lg border border-slate-300 px-3 py-2"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                type="password"
                placeholder="Password"
              />
              <button
                className="rounded-lg bg-blue-700 px-4 py-2 font-semibold text-white hover:bg-blue-800"
                type="submit"
              >
                Sign In
              </button>
            </form>
          </section>
        ) : (
          <section className="mt-10 grid gap-8 lg:grid-cols-[1fr_1.2fr]">
            <form className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm" onSubmit={onCreateJob}>
              <div className="flex items-center justify-between">
                <h2 className="text-xl font-semibold">Create Job</h2>
                <button className="text-sm text-blue-700 hover:text-blue-900" type="button" onClick={onLogout}>
                  Logout
                </button>
              </div>
              <div className="mt-4 grid gap-3">
                <input
                  className="rounded-lg border border-slate-300 px-3 py-2"
                  placeholder="Title"
                  value={newJob.title}
                  onChange={(event) => setNewJob({ ...newJob, title: event.target.value })}
                  required
                />
                <input
                  className="rounded-lg border border-slate-300 px-3 py-2"
                  placeholder="Company"
                  value={newJob.company}
                  onChange={(event) => setNewJob({ ...newJob, company: event.target.value })}
                  required
                />
                <input
                  className="rounded-lg border border-slate-300 px-3 py-2"
                  placeholder="Location"
                  value={newJob.location}
                  onChange={(event) => setNewJob({ ...newJob, location: event.target.value })}
                  required
                />
                <select
                  className="rounded-lg border border-slate-300 px-3 py-2"
                  value={newJob.workMode}
                  onChange={(event) => setNewJob({ ...newJob, workMode: event.target.value })}
                >
                  <option>Remote</option>
                  <option>Hybrid</option>
                  <option>Onsite</option>
                </select>
                <button className="rounded-lg bg-emerald-700 px-4 py-2 font-semibold text-white hover:bg-emerald-800" type="submit">
                  Publish Job
                </button>
              </div>
            </form>

            <div className="grid gap-4">
              {loading && <div className="rounded-lg border border-blue-200 bg-blue-50 p-3 text-blue-900">Loading jobs...</div>}
              {jobs.map((job) => (
                <article key={job.id} className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
                  <h3 className="text-xl font-semibold">{job.title}</h3>
                  <p className="mt-1 text-slate-700">{job.company}</p>
                  <div className="mt-3 flex flex-wrap gap-2 text-sm text-slate-600">
                    <span className="rounded-full bg-slate-100 px-3 py-1">{job.location}</span>
                    <span className="rounded-full bg-slate-100 px-3 py-1">{job.workMode}</span>
                    <span className="rounded-full bg-slate-100 px-3 py-1">Posted: {job.postedOn}</span>
                    <span className="rounded-full bg-slate-100 px-3 py-1">By: {job.createdBy}</span>
                  </div>
                </article>
              ))}
            </div>
          </section>
        )}

        {error && <div className="mt-6 rounded-lg border border-rose-300 bg-rose-50 p-3 text-rose-900">{error}</div>}
      </div>
    </main>
  );
}
