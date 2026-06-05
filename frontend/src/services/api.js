import axios from "axios";

//Edit base URL if needed
const axiosInstance = axios.create({
  baseURL: "https://localhost:7072",
  withCredentials: true
})

export default axiosInstance