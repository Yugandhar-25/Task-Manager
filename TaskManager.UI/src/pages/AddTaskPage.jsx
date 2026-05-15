/* eslint-disable no-unused-vars */
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { createTask } from "../services/taskService";

function AddTaskPage() {
    const [title, setTitle] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!title.trim()) {
            setError("Task title cannot be empty.");
            return;
        }

        setLoading(true);
        setError("");
        try {
            await createTask(title);
            navigate("/tasks");
        } catch (err) {
            setError("Failed to create task.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={styles.container}>
            <div style={styles.header}>
                <h1 style={styles.headerTitle}>✏️ Add New Task</h1>
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
                            disabled={loading}
                        />
                    </div>

                    <div style={styles.actions}>
                        <button
                            type="submit"
                            style={loading ? styles.submitButtonDisabled : styles.submitButton}
                            disabled={loading}
                        >
                            {loading ? "Creating..." : "Create Task"}
                        </button>
                        <button
                            type="button"
                            onClick={() => navigate("/tasks")}
                            style={styles.cancelButton}
                            disabled={loading}
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
    input: {
        width: "100%",
        padding: "10px 14px",
        fontSize: "14px",
        border: "1.5px solid #e2e8f0",
        borderRadius: "8px",
        boxSizing: "border-box",
        fontFamily: "inherit",
    },
    error: {
        backgroundColor: "#fef2f2",
        color: "#dc2626",
        border: "1px solid #fecaca",
        padding: "10px 14px",
        borderRadius: "8px",
        fontSize: "13px",
        marginBottom: "16px",
        textAlign: "center",
        margin: 0,
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

export default AddTaskPage;