#version 330 core
out vec4 FragColor;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 FragPosLightSpace;
} fs_in;
struct PointLightResult {	
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
struct PointLight {
    vec3 lightPos;
    vec3 lightColor;
};
#define NR_POINT_LIGHTS 3

uniform sampler2D diffuseTexture;
uniform sampler2D shadowMap;
uniform vec3 Cubecolor;
uniform vec3 lightPos;
uniform vec3 viewPos;
//生成多个光源
uniform PointLight pointLights[NR_POINT_LIGHTS];

vec3 Calculate(vec3 normal,PointLight pointLight){
    vec3 lightColor = pointLight.lightColor;
    // ambient
    vec3 ambient = 0.3 * lightColor;
    // diffuse
    vec3 lightDir = normalize(pointLight.lightPos - fs_in.FragPos);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * lightColor;
    // specular
    vec3 ress = vec3(1.0,0.0,0.0);
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
    vec3 specular = spec * lightColor;    
    // calculate shadow
    //vec3 lighting = (ambient +diffuse + specular) * color;   
    
    vec3 res;
    //res = (ambient+diffuse+specular);
    res = (diffuse);
    vec3 add = vec3(0.3,0.1,0.7);
    //res = (ambient+diffuse+specular);
    return res;
}
float ShadowCalculation(vec4 fragPosLightSpace)
{
    // 已经在标准视见体中？//转为三维时要注意w的值w为1才有意义
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // transform to [0,1] range
    projCoords = projCoords * 0.5 + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    float closestDepth = texture(shadowMap, projCoords.xy).r; 
    //float closestDepth2 = texture(shadowMap, projCoords.xy).r; 
    // get depth of current fragment from light's perspective
    float currentDepth = projCoords.z;
    // 如果当前值大于深度值则是阴影 ps 深度值越远越大
    float shadow = currentDepth > closestDepth  ? 1.0 : 0.0;

    return shadow;
}
void main()
{   
    vec3 result = vec3(0);
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightColor = vec3(0.3);
    for(int i = 0; i < NR_POINT_LIGHTS; i++){
        result += Calculate(normal,pointLights[i]); 
    }
    // calculate shadow
    //float shadow = ShadowCalculation(fs_in.FragPosLightSpace);

    //vec3 lighting = (ambient +diffuse + specular) * color;   
    //vec3 lighting = (diffuse ) * color;                  
    //vec3 lighting = (ambient + (1.0 - shadow) * (diffuse + specular)) * color;    
    
    FragColor = vec4(result*Cubecolor, 1.0) ;
}