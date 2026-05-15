/* eslint-disable no-unused-vars */
import { useNavigate, Link } from "react-router-dom";
import { useState } from "react";
import { register } from "../services/authService";

function RegisterPage(){
    const[username, SetUsername] = useState("");
    const[password, SetPassword] = useState("");
    const[error, SetError] = useState("");
    const[loading, SetLoading] = useState(false);

    const navigate = useNavigate();

    const handleRegister = async(e)=>{
        e.preventDefault();
        SetError("");
        SetLoading(true);

        try{
            await register(username, password);
            navigate("/login");
        }
        catch(err){
            SetError("Invalid credentials");
        }
        finally{
            SetLoading(false);
        }
    }

    return(
        <div style={styles.container}>
            <div style={styles.card}>
                <h2 style={styles.title}>Task Manager</h2>
                <p style={styles.subtitle}>Create your account</p>
                {error && <p style={styles.error}>{error}</p>}

                <form onSubmit={handleRegister}>
                    <div style={styles.field}>
                        <label style={styles.label}>Username</label>
                        <input style={styles.input} type="text" value={username}
                            onChange={(e)=>SetUsername(e.target.value)} placeholder="Enter Username" required/>
                    </div>
                    <div style={styles.field}>
                        <label style={styles.label}>Password</label>
                        <input style={styles.input} type="password" value={password}
                            onChange={(e)=>SetPassword(e.target.value)} placeholder="Enter Password" required/>
                    </div>

                    <button style={loading ? styles.buttonDisabled : styles.button}
                        type="submit" disabled={loading}>{loading ? "Signing up.." : "Register"}</button>
                </form>
                <p style={styles.footer}>Already have an account?{" "}
                    <Link to="/login" style={styles.link}>Login</Link>
                </p>
            </div>
        </div>
    )
}

const styles = {
    container: {
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        height: "100vh",
        background: "linear-gradient(135deg, #ede9fe, #e0e7ff)",
    },
    card: {
        backgroundColor: "white",
        padding: "40px",
        borderRadius: "16px",
        boxShadow: "0 8px 32px rgba(79, 70, 229, 0.12)",
        width: "100%",
        maxWidth: "400px",
    },
    title: {
        textAlign: "center",
        marginBottom: "4px",
        color: "#1e1b4b",
        fontSize: "26px",
        fontWeight: "700",
        letterSpacing: "-0.5px",
    },
    subtitle: {
        textAlign: "center",
        color: "#6b7280",
        marginBottom: "28px",
        fontSize: "14px",
    },
    field: {
        marginBottom: "18px",
    },
    label: {
        display: "block",
        marginBottom: "6px",
        fontSize: "13px",
        fontWeight: "600",
        color: "#374151",
        textTransform: "uppercase",
        letterSpacing: "0.05em",
    },
    input: {
        width: "100%",
        padding: "11px 14px",
        borderRadius: "10px",
        border: "1.5px solid #e5e7eb",
        fontSize: "14px",
        boxSizing: "border-box",
        outline: "none",
        transition: "border-color 0.2s",
        color: "#111827",
        backgroundColor: "#f9fafb",
    },
    button: {
        width: "100%",
        padding: "12px",
        backgroundColor: "#4f46e5",
        color: "white",
        border: "none",
        borderRadius: "10px",
        fontSize: "15px",
        fontWeight: "600",
        cursor: "pointer",
        marginTop: "8px",
        letterSpacing: "0.02em",
        transition: "background-color 0.2s, transform 0.1s",
    },
    buttonDisabled: {
        width: "100%",
        padding: "12px",
        backgroundColor: "#c7d2fe",
        color: "white",
        border: "none",
        borderRadius: "10px",
        fontSize: "15px",
        fontWeight: "600",
        cursor: "not-allowed",
        marginTop: "8px",
        letterSpacing: "0.02em",
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
    footer: {
        textAlign: "center",
        marginTop: "22px",
        fontSize: "14px",
        color: "#6b7280",
    },
    link: {
        color: "#4f46e5",
        textDecoration: "none",
        fontWeight: "600",
    },
};

export default RegisterPage;