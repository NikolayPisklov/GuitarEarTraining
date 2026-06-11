import axiosInstance from "./api";

export async function addExerciseAttempt(answers, exerciseId){
  const dto = {
    Answers: answers,
    ExerciseId: exerciseId,
  }
  await axiosInstance.post('/exerciseResult/addAttempt', dto)
}
export async function getLatestScore(exerciseId)
{
  const response = await axiosInstance.get('/exerciseResult/getLatestAttemptScore', 
    {params: {exerciseId: exerciseId}})
  return response.data
}
