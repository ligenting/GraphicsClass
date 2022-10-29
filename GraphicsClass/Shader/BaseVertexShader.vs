#version 330 core
layout (location = 0) in vec3 aPos;
void main()
{
    vs_out.FragPos = vec3(model * vec4(aPos, 1.0));//世界顶点
    vs_out.Normal = transpose(inverse(mat3(model))) * aNormal;//不等比缩放时或出现法线错误
    vs_out.TexCoords = aTexCoords;
    vs_out.FragPosLightSpace = lightSpaceMatrix * vec4(vs_out.FragPos, 1.0);//世界顶点转为灯光坐标系
    gl_Position = projection * view * model * vec4(aPos, 1.0);//视口坐标
}
