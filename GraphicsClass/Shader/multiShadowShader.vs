#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec2 TexCoords;

out VS_OUT {
    vec3 FragPos;//世界坐标
    vec3 Normal;//法线坐标
    vec2 TexCoords;
    vec4 FragPosLightSpace1;
    vec4 FragPosLightSpace2;
} vs_out;

uniform mat4 projection;//投影矩阵
uniform mat4 view;//观察矩阵
uniform mat4 model;//世界坐标变换矩阵
uniform mat4 lightSpaceMatrix1;//
uniform mat4 lightSpaceMatrix2;//

void main()
{
    vs_out.FragPos = vec3(model * vec4(aPos, 1.0));//世界顶点
    vs_out.Normal = transpose(inverse(mat3(model))) * aNormal;//不等比缩放时或出现法线错误
    vs_out.TexCoords = aTexCoords;
    vs_out.FragPosLightSpace1 = lightSpaceMatrix1 * vec4(vs_out.FragPos, 1.0);//世界顶点转为灯光坐标系
    vs_out.FragPosLightSpace2 = lightSpaceMatrix2 * vec4(vs_out.FragPos, 1.0);
    gl_Position = projection * view * model * vec4(aPos, 1.0);//视口坐标
}