/* eslint-disable no-unused-vars */
import { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getTaskById, updateTask } from "../services/taskService";

function UpdateTaskPage() {
    const { id } = useParams();
    const navigate = useNavigate();
    const [title, setTitle] = useState("");
    const [isCompleted, setIsCompleted] = useState(false);
    const [loading, setLoading] = useState(true);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState("");

    useEffect(() => {
        const fetchTask = async () => {
            try {
                const task = await getTaskById(id);
                setTitle(task.title);
                setIsCompleted(task.isCompleted);
            } catch (err) {
                setError("Failed to load task.");
            } finally {
                setLoading(false);
            }
        };

        fetchTask();
    }, [id]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!title.trim()) {
            setError("Task title cannot be empty.");
            return;
        }

        setSubmitting(true);
        setError("");
        try {
            await updateTask(id, title, isCompleted);
            navigate("/tasks");
        } catch (err) {
            setError("Failed to update task.");
        } finally {
            setSubmitting(false);
        }
    };

    if (loading) {
        return (
            <div style={styles.container}>
                <div style={styles.header}>
                    <h1 style={styles.headerTitle}>✏️ Edit Task</h1>
                </div>
                <div style={styles.content}>
                    <p style={styles.info}>Loading task...</p>
                </div>
            </div>
        );
    }

    return (
        <div style={styles.container}>
            <div style={styles.header}>
                <h1 style={styles.headerTitle}>✏️ Edit Task</h1>
            </div>

            <div style={styles.content}>
                <form onSubmit={handleSubmit} style={styles.form}>
                    {error && <p style={styles.error}>{error}</p>}

                    <div style={styles.formGroup}>
                        <label style={styles.label}>Task Title</label>
                        <input
                            type="text"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            placeholder="Enter task title..."
                            style={styles.input}
                            disabled={submitting}
                        />
                    </div>

                    <div style={styles.formGroup}>
                        <label style={styles.checkboxLabel}>
                            <input
                                type="checkbox"
                                checked={isCompleted}
                                onChange={(e) => setIsCompleted(e.target.checked)}
                                disabled={submitting}
                                style={styles.checkbox}
                            />
                            {isCompleted ? "Completed" : "Mark as Completed"}
                        </label>
                    </div>

                    <div style={styles.actions}>
                        <button
                            type="submit"
                            style={submitting ? styles.submitButtonDisabled : styles.submitButton}
                            disabled={submitting}
                        >
                            {submitting ? "Updating..." : "Update Task"}
                        </button>
                        <button
                            type="button"
                            onClick={() => navigate("/tasks")}
                            style={styles.cancelButton}
                            disabled={submitting}
                        >
                            Cancel
                        </button>
                    </div>
                </form>
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
    content: {
        maxWidth: "500px",
        margin: "40px auto",
        padding: "0 16px",
    },
    form: {
        backgroundColor: "white",
        borderRadius: "12px",
        padding: "32px",
        boxShadow: "0 2px 12px rgba(79, 70, 229, 0.1)",
    },
    formGroup: {
        marginBottom: "24px",
    },
    label: {
        display: "block",
        fontSize: "14px",
        fontWeight: "600",
        color: "#374151",
        marginBottom: "8px",
    },
    checkboxLabel: {
        display: "flex",
        alignItems: "center",
        fontSize: "14px",
        fontWeight: "500",
        color: "#374151",
        cursor: "pointer",
    },
    checkbox: {
        marginRight: "8px",
        cursor: "pointer",
        width: "18px",
        height: "18px",
    },
    input: {
        width: "100%",
        padding: "10px 14px",
        fontSize: "14px",
        border: "1.5px solid #e2e8f0",
        borderRadius: "8px",
        boxSizing: "border-box",
        fontFamily: "inherit",
    },
    info: {
        textAlign: "center",
        color: "#6b7280",
        fontSize: "14px",
        marginTop: "40px",
    },
    error: {
        backgroundColor: "#fef2f2",
        color: "#dc2626",
        border: "1px solid #fecaca",
        padding: "10px 14px",
        borderRadius: "8px",
        fontSize: "13px",
        textAlign: "center",
        margin: 0,
        marginBottom: "16px",
    },
    actions: {
        display: "flex",
        gap: "12px",
        justifyContent: "flex-end",
    },
    submitButton: {
        padding: "10px 24px",
        backgroundColor: "#4f46e5",
        color: "white",
        border: "none",
        borderRadius: "8px",
        fontSize: "14px",
        fontWeight: "600",
        cursor: "pointer",
    },
    submitButtonDisabled: {
        padding: "10px 24px",
        backgroundColor: "#a5b4fc",
        color: "white",
        border: "none",
        borderRadius: "8px",
        fontSize: "14px",
        fontWeight: "600",
        cursor: "not-allowed",
    },
    cancelButton: {
        padding: "10px 24px",
        backgroundColor: "#f1f5f9",
        color: "#475569",
        border: "1.5px solid #e2e8f0",
        borderRadius: "8px",
        fontSize: "14px",
        fontWeight: "600",
        cursor: "pointer",
    },
};

export default UpdateTaskPage;