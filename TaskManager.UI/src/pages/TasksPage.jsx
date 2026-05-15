/* eslint-disable no-unused-vars */
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { createTask, deleteTask, getTasks } from "../services/taskService";
import { logout } from "../services/authService";

function TasksPage() {

    const [tasks, setTasks] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [refresh, setRefresh] = useState(0);
    const PAGE_SIZE = 10;

    const navigate = useNavigate();

    useEffect(() => {
        let cancelled = false;

        const fetchTasks = async () => {
            setLoading(true);
            setError("");
            try {
                const data = await getTasks(page, PAGE_SIZE);
                if (cancelled) return;
                if (Array.isArray(data)) {
                    setTasks(data);
                    setTotalPages(1);
                } else {
                    setTasks(data.items ?? []);
                    setTotalPages(data.totalPages ?? 1);
                }
            } catch (err) {
                if (!cancelled) setError("Failed to load tasks.");
            } finally {
                if (!cancelled) setLoading(false);
            }
        };

        fetchTasks();
        return () => { cancelled = true; };
    }, [page, refresh]);

    const handleDelete = async (id) => {
        try {
            await deleteTask(id);
            setRefresh((r) => r + 1); // trigger re-fetch without changing page
        } catch (err) {
            setError("Failed to delete task.");
        }
    };

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    return (
        <div style={styles.container}>

            {/* Header */}
            <div style={styles.header}>
                <h1 style={styles.headerTitle}>📋 My Tasks</h1>
                <div style={styles.headerActions}>
                    <button onClick={() => navigate("/tasks/add")} style={styles.addButton}>+ Add Task</button>
                    <button onClick={handleLogout} style={styles.logoutButton}>Logout</button>
                </div>
            </div>

            {/* Main Content */}
            <div style={styles.content}>

                {/* Error */}
                {error && <p style={styles.error}>{error}</p>}

                {/* Task List */}
                {loading ? (
                    <p style={styles.info}>Loading tasks...</p>
                ) : tasks.length === 0 ? (
                    <div style={styles.emptyState}>
                        <p style={styles.emptyIcon}>🗒️</p>
                        <p style={styles.emptyText}>No tasks yet.</p>
                        <p style={styles.emptySubText}>Click <strong>+ Add Task</strong> to get started.</p>
                    </div>
                ) : (
                    <ul style={styles.list}>
                        {tasks.map((task) => (
                            <li key={task.id} style={styles.taskItem}>
                                <div style={styles.taskRow}>
                                    {/* Status badge */}
                                    <span style={task.isCompleted ? styles.badgeDone : styles.badgePending}>
                                        {task.isCompleted ? "Done" : "Pending"}
                                    </span>

                                    {/* Task info */}
                                    <div style={styles.taskInfo}>
                                        <span style={task.isCompleted ? styles.taskTitleDone : styles.taskTitle}>
                                            {task.title}
                                        </span>
                                        <span style={styles.taskDate}>
                                            Created {new Date(task.createdAt).toLocaleDateString()}
                                        </span>
                                    </div>

                                    {/* Actions */}
                                    <div style={styles.taskActions}>
                                        <button
                                            onClick={() => navigate(`/tasks/${task.id}/edit`)}
                                            style={styles.editButton}
                                        >
                                            Edit
                                        </button>
                                        <button
                                            onClick={() => handleDelete(task.id)}
                                            style={styles.deleteButton}
                                        >
                                            Delete
                                        </button>
                                    </div>
                                </div>
                            </li>
                        ))}
                    </ul>
                )}

                {/* Pagination */}
                {totalPages > 1 && (
                    <div style={styles.pagination}>
                        <button
                            onClick={() => setPage((p) => Math.max(p - 1, 1))}
                            disabled={page === 1}
                            style={page === 1 ? styles.pageButtonDisabled : styles.pageButton}
                        >
                            ← Prev
                        </button>
                        <span style={styles.pageInfo}>Page {page} of {totalPages}</span>
                        <button
                            onClick={() => setPage((p) => Math.min(p + 1, totalPages))}
                            disabled={page === totalPages}
                            style={page === totalPages ? styles.pageButtonDisabled : styles.pageButton}
                        >
                            Next →
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
}

const styles = {
    container: {
        minHeight: "100vh",
        background: "linear-gradient(135deg, #ede9fe, #e0e7ff)",
    },
    header: {
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        padding: "18px 32px",
        backgroundColor: "white",
        boxShadow: "0 2px 12px rgba(79, 70, 229, 0.1)",
        position: "sticky",
        top: 0,
        zIndex: 10,
    },
    headerTitle: {
        fontSize: "22px",
        fontWeight: "700",
        color: "#1e1b4b",
        letterSpacing: "-0.5px",
        margin: 0,
    },
    headerActions: {
        display: "flex",
        gap: "10px",
    },
    addButton: {
        padding: "8px 18px",
        backgroundColor: "#4f46e5",
        color: "white",
        border: "none",
        borderRadius: "8px",
        fontSize: "14px",
        fontWeight: "600",
        cursor: "pointer",
    },
    logoutButton: {
        padding: "8px 18px",
        backgroundColor: "#f1f5f9",
        color: "#475569",
        border: "1.5px solid #e2e8f0",
        borderRadius: "8px",
        fontSize: "14px",
        fontWeight: "600",
        cursor: "pointer",
    },
    content: {
        maxWidth: "720px",
        margin: "40px auto",
        padding: "0 16px",
    },
    error: {
        backgroundColor: "#fef2f2",
        color: "#dc2626",
        border: "1px solid #fecaca",
        padding: "10px 14px",
        borderRadius: "10px",
        fontSize: "13px",
        marginBottom: "16px",
        textAlign: "center",
    },
    info: {
        textAlign: "center",
        color: "#6b7280",
        fontSize: "14px",
        marginTop: "40px",
    },
    emptyState: {
        textAlign: "center",
        marginTop: "80px",
    },
    emptyIcon: {
        fontSize: "48px",
        margin: "0 0 12px",
    },
    emptyText: {
        fontSize: "18px",
        fontWeight: "600",
        color: "#374151",
        margin: "0 0 6px",
    },
    emptySubText: {
        fontSize: "14px",
        color: "#6b7280",
        margin: 0,
    },
    list: {
        listStyle: "none",
        padding: 0,
        margin: 0,
        display: "flex",
        flexDirection: "column",
        gap: "10px",
    },
    taskItem: {
        backgroundColor: "white",
        borderRadius: "12px",
        padding: "14px 18px",
        boxShadow: "0 2px 8px rgba(79, 70, 229, 0.07)",
        transition: "box-shadow 0.2s",
    },
    taskRow: {
        display: "flex",
        alignItems: "center",
        gap: "14px",
    },
    badgePending: {
        fontSize: "11px",
        fontWeight: "700",
        padding: "3px 10px",
        borderRadius: "20px",
        backgroundColor: "#fef9c3",
        color: "#854d0e",
        flexShrink: 0,
        textTransform: "uppercase",
        letterSpacing: "0.04em",
    },
    badgeDone: {
        fontSize: "11px",
        fontWeight: "700",
        padding: "3px 10px",
        borderRadius: "20px",
        backgroundColor: "#dcfce7",
        color: "#166534",
        flexShrink: 0,
        textTransform: "uppercase",
        letterSpacing: "0.04em",
    },
    taskInfo: {
        flex: 1,
        display: "flex",
        flexDirection: "column",
        gap: "2px",
    },
    taskTitle: {
        fontSize: "15px",
        fontWeight: "500",
        color: "#111827",
    },
    taskTitleDone: {
        fontSize: "15px",
        fontWeight: "500",
        color: "#9ca3af",
        textDecoration: "line-through",
    },
    taskDate: {
        fontSize: "12px",
        color: "#9ca3af",
    },
    taskActions: {
        display: "flex",
        gap: "8px",
        flexShrink: 0,
    },
    editButton: {
        padding: "5px 14px",
        backgroundColor: "#f1f5f9",
        color: "#475569",
        border: "1.5px solid #e2e8f0",
        borderRadius: "7px",
        fontSize: "12px",
        fontWeight: "600",
        cursor: "pointer",
    },
    deleteButton: {
        padding: "5px 14px",
        backgroundColor: "#fef2f2",
        color: "#dc2626",
        border: "1.5px solid #fecaca",
        borderRadius: "7px",
        fontSize: "12px",
        fontWeight: "600",
        cursor: "pointer",
    },
    pagination: {
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        gap: "16px",
        marginTop: "32px",
        paddingBottom: "40px",
    },
    pageButton: {
        padding: "8px 18px",
        backgroundColor: "white",
        color: "#4f46e5",
        border: "1.5px solid #a5b4fc",
        borderRadius: "8px",
        fontSize: "14px",
        fontWeight: "600",
        cursor: "pointer",
    },
    pageButtonDisabled: {
        padding: "8px 18px",
        backgroundColor: "#f1f5f9",
        color: "#9ca3af",
        border: "1.5px solid #e2e8f0",
        borderRadius: "8px",
        fontSize: "14px",
        fontWeight: "600",
        cursor: "not-allowed",
    },
    pageInfo: {
        fontSize: "14px",
        color: "#6b7280",
        fontWeight: "500",
    },
};

export default TasksPage;