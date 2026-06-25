import axiosInstance from "./api";

export async function getPitchSamples(){
    const response = await axiosInstance.get("/pitch/get-samples")
    return response.data
}
export async function getAnswerOptionsForPitch(){
    const response = await axiosInstance.get("/pitch/get-answer-options")
    return response.data
}