import axios from "axios";

//Edit base URL if needed
const instance = axios.create({
    baseURL: "https://localhost:7072",
    withCredentials: true
})

export default instance