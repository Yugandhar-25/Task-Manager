/* eslint-disable no-unused-vars */
import axios from "axios";
import { getToken } from "./authService";

const API_URL = "https://localhost:7288/api/Tasks";

const authHeaders = () => ({
    headers: {Authorization: `Bearer ${getToken()}`}
});

export const getTasks = async(page=1, pageSize=10)=>{
    const response = await axios.get(`${API_URL}?page=${page}&pageSize=${pageSize}`, authHeaders());
    return response.data;
}

export const getTaskById = async(id)=>{
    const response = await axios.get(`${API_URL}/${id}`, authHeaders());
    return response.data;
}

export const createTask = async(title)=>{
    const response = await axios.post(API_URL, {title}, authHeaders());
    return response.data;
}

export const updateTask = async(id, title, isCompleted)=>{
    const response = await axios.put(`${API_URL}/${id}`, {title, isCompleted}, authHeaders());
    return response.data;
}

export const deleteTask = async(id)=>{
    const response = await axios.delete(`${API_URL}/${id}`, authHeaders());
    return response.data;
}

