import axios from "axios";

const API_URL = "https://localhost:7288/api/Auth";

export const login = async(username, password) => {
    const response = await axios.post(`${API_URL}/login`, {username, password});
    localStorage.setItem("token", response.data.token);
    return response.data;
}

export const register = async(username, password) => {
    const response = await axios.post(`${API_URL}/register`, {username, password});
    localStorage.setItem("token", response.data.token);
}

export const logout = ()=>{
    localStorage.removeItem("token");
}

export const getToken =  ()=>localStorage.getItem("token");

